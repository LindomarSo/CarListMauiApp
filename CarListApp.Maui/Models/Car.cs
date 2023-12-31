﻿using SQLite;

namespace CarListApp.Maui.Models;

[Table("cars")]
public class Car : BaseEntity
{
    public string Make { get; set; }
    public string Model { get; set; }

    [MaxLength(10)]
    [Unique]
    public string Vin { get; set; }
}
