// In Services/MauiNavigationService.cs
namespace DriverLogisticsApp.Services
{
    public class MauiNavigationService : INavigationService
    {
        public Task GoBackAsync()
        {
            return Shell.Current.GoToAsync("..");
        }

        public Task NavigateToAsync(string route)
        {
            return Shell.Current.GoToAsync(route);
        }

        public Task NavigateToAsync(string route, IDictionary<string, object> parameters)
        {
            return Shell.Current.GoToAsync(route, parameters);
        }
    }
}