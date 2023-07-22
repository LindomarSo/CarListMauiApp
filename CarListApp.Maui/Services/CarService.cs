using CarListApp.Maui.Models;
using SQLite;

namespace CarListApp.Maui.Services
{
    public class CarService
    {
        private SQLiteConnection conn;
        string _dbPath;
        int result = 0;

        public string StatusMessage { get; private set; }

        public CarService(string dbPath)
        {
            _dbPath = dbPath;
        }

        public List<Car> GetCars()
        {
            try
            {
                Init();

                return conn.Table<Car>().ToList();
            }
            catch (Exception)
            {
                StatusMessage = "Failed to retrieve data.";
            }

            return new();
        }

        private void Init()
        {
            if (conn is not null) return;

            conn = new SQLiteConnection(_dbPath);
            conn.CreateTable<Car>();
        }

        public void AddCar(Car car)
        {
            try
            {
                Init();

                if (car is null)
                    throw new Exception("Invalid car Record");

                result = conn.Insert(car);
                StatusMessage = result == 0 ? "Insert Failed" : "Insert Successful";
            }
            catch (Exception)
            {
                StatusMessage = "Failed to insert data.";
            }
        }

        public int DeleteCar(int id)
        {
            try
            {
                Init();
                return conn.Table<Car>().Delete(q => q.Id == id);
            }
            catch (Exception)
            {
                StatusMessage = "Failed to delete data.";
            }

            return 0;
        }

        public Car GetCar(int id)
        {
            try
            {
                Init();
                return conn.Table<Car>().FirstOrDefault(c => c.Id == id);
            }
            catch (Exception)
            {
                StatusMessage = "Failed to retrieve data.";
            }

            return new Car();
        }

        public void UpdateCar(Car car)
        {
            try
            {
                Init();

                if (car is null)
                    throw new Exception("Invalid car Record");

                result = conn.Update(car);
                StatusMessage = result > 0 ? "Car updated" : "Failed to update";
            }
            catch (Exception)
            {
                StatusMessage = "Failed to delete data.";
            }
        }
    }
}
