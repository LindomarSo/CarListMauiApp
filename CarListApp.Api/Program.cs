using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

//var dbPath = Path.Join(Directory.GetCurrentDirectory(), "carlist.db");
var conn = new SqliteConnection($@"Data source=D:\.NET Maui\Databases\carlist.db");
builder.Services.AddDbContext<CarListDbContext>(options => options.UseSqlite(conn));

builder.Services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CarListDbContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/cars", async (CarListDbContext db) => await db.Cars.ToListAsync());
app.MapGet("/cars/{id}", async (int id, CarListDbContext db) => await db.Cars.FirstOrDefaultAsync(x => x.Id == id) is Car car ? Results.Ok(car) : Results.NotFound());

app.MapPut("/cars/{id}", async (int id, [FromBody] Car car, CarListDbContext db) =>
{
    var record = await db.Cars.FirstOrDefaultAsync(x => x.Id == id);

    if (record is null)
        return Results.NotFound();

    record.Vin = car.Vin;
    record.Make = car.Make;
    record.Model = car.Model;

    db.SaveChanges();

    return Results.NoContent();
});

app.MapDelete("/cars/{id}", async (int id, CarListDbContext db) =>
{
    var record = await db.Cars.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    if (record is null)
        return Results.NotFound();

    db.Remove(record);
    db.SaveChanges();

    return Results.NoContent();
});

app.MapPost("/cars", async (Car car, CarListDbContext db) =>
{
    await db.AddAsync(car);
    db.SaveChanges();

    return Results.Created($"/cars/{car.Id}", car);
});

app.MapPost("/login", async ([FromBody]LoginDto login, CarListDbContext db, UserManager<IdentityUser> _userManager) =>
{
    var user = await _userManager.FindByNameAsync(login.UserName);  

    if(user is null)
    {
        return Results.Unauthorized();
    }

    if(!await _userManager.CheckPasswordAsync(user, login.Password))
    {
        return Results.Unauthorized();
    }

    var response = new AuthResponseDto
    {
        UserId = user.Id,
        Username = login.UserName,
        Token = ""
    };

    return Results.Ok(response);
});

app.UseCors("CorsPolicy");

app.Run();

internal class LoginDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

internal class AuthResponseDto
{
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Token { get; set; }
}
