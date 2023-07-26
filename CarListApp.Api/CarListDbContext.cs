using Microsoft.EntityFrameworkCore;

public class CarListDbContext : DbContext
{
    public CarListDbContext(DbContextOptions<CarListDbContext> options) : base(options)
    {
        
    }

    public DbSet<Car> Cars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>().HasData(
            new Car(1, "Honda", "Fit", "2234-JDS"),
            new Car(2, "Honda", "Civic", "6534-JDP"),
            new Car(3, "Toyota", "Etios", "6436-JTS"),
            new Car(4, "Fiat", "Estrada", "0983-ODS"),
            new Car(5, "Fiat", "Uno", "6034-LDS")
        );
    }
}