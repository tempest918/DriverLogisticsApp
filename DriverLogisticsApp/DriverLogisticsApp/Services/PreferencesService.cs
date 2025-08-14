using Microsoft.Maui.ApplicationModel;

namespace DriverLogisticsApp.Services
{
    public class PreferencesService : IPreferences
    {
        public T Get<T>(string key, T defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Set<T>(string key, T value)
        {
            Preferences.Set(key, value);
        }
    }
}