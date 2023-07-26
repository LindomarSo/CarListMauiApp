public class Car
{
    public Car()
    {
        
    }

    public Car(int id, string make, string model, string vin)
    {
        Id = id;
        Make = make;
        Model = model;
        Vin = vin;
    }

    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
}