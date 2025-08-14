namespace DriverLogisticsApp.Services
{
    public interface IPreferences
    {
        bool Get(string key, bool defaultValue);
    }
}