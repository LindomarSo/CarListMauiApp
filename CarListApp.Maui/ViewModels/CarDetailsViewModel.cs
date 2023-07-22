using CarListApp.Maui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Web;

namespace CarListApp.Maui.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class CarDetailsViewModel : BaseViewModel, IQueryAttributable
{
    [ObservableProperty]
    Car car;

    [ObservableProperty]
    int id;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Id = Convert.ToInt32(HttpUtility.UrlDecode(query[nameof(Id)].ToString()));
        Car = App.CarService.GetCar(Id);
    }
}
