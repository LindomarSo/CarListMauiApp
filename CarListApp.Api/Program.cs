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

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.MapGet("/cars", async (CarListDbContext db) => await db.Cars.ToListAsync());
app.MapGet("/cars/{id}", async (int id, CarListDbContext db) =>
        await db.Cars.FirstOrDefaultAsync(x => x.Id == id) is Car car ? Results.Ok(car) : Results.NotFound());

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

//app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.Run();
