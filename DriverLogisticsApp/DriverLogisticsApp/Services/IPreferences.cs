namespace DriverLogisticsApp.Services
{
    public interface IPreferences
    {
        bool Get(string key, bool defaultValue);
        void Set(string key, bool value);
        string Get(string key, string defaultValue);
        void Set(string key, string value);
    }
}