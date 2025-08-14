using Microsoft.Maui.ApplicationModel;

namespace DriverLogisticsApp.Services
{
    public class PreferencesService : IPreferences
    {
        public bool Get(string key, bool defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Set(string key, bool value)
        {
            Preferences.Set(key, value);
        }

        public string Get(string key, string defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Set(string key, string value)
        {
            Preferences.Set(key, value);
        }
    }
}