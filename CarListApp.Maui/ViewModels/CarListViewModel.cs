using CarListApp.Maui.Models;
using CarListApp.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CarListApp.Maui.ViewModels;

public partial class CarListViewModel : BaseViewModel
{
    const string editButtonText = "Update Car";
    const string createButtonText = "Add Car";
    public ObservableCollection<Car> Cars { get; private set; } = new();

    public CarListViewModel()
    {
        Title = "Car List";
        AddEditButtonText = createButtonText;
        GetCarList().Wait();
    }

    [ObservableProperty]
    string addEditButtonText;

    [ObservableProperty]
    bool isRefreshing;

    [ObservableProperty]
    string model;

    [ObservableProperty]
    string make;

    [ObservableProperty]
    string vin;

    [ObservableProperty]
    int carId;

    [RelayCommand]
    async Task GetCarList()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;

            if (Cars.Any())
                Cars.Clear();

            var cars = App.CarService.GetCars();

            foreach (var car in cars)
                Cars.Add(car);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get cars: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "Failed to retrive list of cars.", "Ok");
        }
        finally
        {
            IsLoading = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    async Task GetCarDetails(int id)
    {
        if (id == 0) return;

        await Shell.Current.GoToAsync($"{nameof(CarDetailsPage)}?Id={id}", true);
    }

    [RelayCommand]
    async Task SaveCar()
    {
        if (string.IsNullOrEmpty(Make) || string.IsNullOrEmpty(Model) || string.IsNullOrEmpty(Vin))
        {
            await Shell.Current.DisplayAlert("Invalid Data", "Please insert valid data", "Ok");
            return;
        }

        var car = new Car
        {
            Make = Make,
            Model = Model,
            Vin = Vin,
        };

        if(CarId != 0)
        {
            car.Id = CarId;
            App.CarService.UpdateCar(car);
        }
        else
        {
            App.CarService.AddCar(car);
        }

        await Shell.Current.DisplayAlert("Info", App.CarService.StatusMessage, "Ok");
        await GetCarList();
        ClearForm();
    }

    [RelayCommand]
    async Task DeleteCar(int id)
    {
        if (id == 0)
        {
            await Shell.Current.DisplayAlert("Invalid Data", "Please insert valid data", "Ok");
            return;
        }

        var result = App.CarService.DeleteCar(id);
        if (result == 0)
        {
            await Shell.Current.DisplayAlert("Failed", "Please insert valid data", "Ok");
        }
        else
        {
            await Shell.Current.DisplayAlert("Deletion Successful", "Record Removed Successfull", "Ok");
            await GetCarList();
        }
    }

    [RelayCommand]
    void SetEditMode(int id)
    {
        AddEditButtonText = editButtonText;
        CarId = id;
        var car = App.CarService.GetCar(id);
        Make = car.Make;
        Model = car.Model;
        Vin = car.Vin;
    }

    [RelayCommand]
    void UpdateCar(int id)
    {
        AddEditButtonText = editButtonText;
        return;
    }

    [RelayCommand]
    void ClearForm()
    {
        AddEditButtonText = createButtonText;
        CarId = 0;
        Make = string.Empty;
        Model= string.Empty;
        Vin= string.Empty;
    }
}
