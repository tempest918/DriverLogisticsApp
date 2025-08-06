namespace DriverLogisticsApp.Services
{
    public interface INavigationService
    {
        Task GoBackAsync();
        Task NavigateToAsync(string route);
        Task NavigateToAsync(string route, IDictionary<string, object> parameters);
    }
}