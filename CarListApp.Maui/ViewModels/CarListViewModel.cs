using CarListApp.Maui.Models;
using CarListApp.Maui.Services;
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
    private readonly CarApiService _apiService;
    NetworkAccess accessType = Connectivity.Current.NetworkAccess;
    string message = string.Empty;

    public ObservableCollection<Car> Cars { get; private set; } = new();

    public CarListViewModel(CarApiService apiService)
    {
        Title = "Car List";
        AddEditButtonText = createButtonText;
        _apiService = apiService;
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

            if (Cars.Any()) Cars.Clear();

            var cars = new List<Car>();

            if (accessType == NetworkAccess.Internet)
            {
                cars = await _apiService.GetCars();
            }
            else
            {
                cars = App.CarService.GetCars();
            }

            foreach (var car in cars) Cars.Add(car);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get cars: {ex.Message}");
            await ShowAlert("Failed to retrive list of cars.");
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
            await ShowAlert("Please insert valid data");
            return;
        }

        var car = new Car
        {
            Make = Make,
            Model = Model,
            Vin = Vin,
        };

        if (CarId != 0)
        {
            car.Id = CarId;
            if (accessType == NetworkAccess.Internet)
            {
                await _apiService.UpdateCar(CarId, car);
                message = _apiService.StatusMessage;
            }
            else
            {
                App.CarService.UpdateCar(car);
                message = App.CarService.StatusMessage;
            }
        }
        else
        {
            if (accessType == NetworkAccess.Internet)
            {
                await _apiService.AddCar(car);
                message = _apiService.StatusMessage;
            }
            else
            {
                App.CarService.AddCar(car);
                message = App.CarService.StatusMessage;
            }
        }

        await ShowAlert(message);
        await GetCarList();
        ClearForm();
    }

    [RelayCommand]
    async Task DeleteCar(int id)
    {
        if (id == 0)
        {
            await ShowAlert("Please insert valid data");
            return;
        }

        if (accessType == NetworkAccess.Internet)
        {
            await _apiService.DeleteCar(id);
            message = _apiService.StatusMessage;
        }
        else
        {
            App.CarService.DeleteCar(id);
            message = App.CarService.StatusMessage;
        }

        await ShowAlert(message);
        await GetCarList();
    }

    [RelayCommand]
    async Task SetEditMode(int id)
    {
        AddEditButtonText = editButtonText;
        CarId = id;
        var car = await _apiService.GetCar(id);
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
        Model = string.Empty;
        Vin = string.Empty;
    }

    private async Task ShowAlert(string message)
        => await Shell.Current.DisplayAlert("Info", message, "Ok");
}
