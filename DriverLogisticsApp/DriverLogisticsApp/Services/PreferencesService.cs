using Microsoft.Maui.ApplicationModel;

namespace DriverLogisticsApp.Services
{
    public class PreferencesService : IPreferences
    {
        public bool Get(string key, bool defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }
    }
}