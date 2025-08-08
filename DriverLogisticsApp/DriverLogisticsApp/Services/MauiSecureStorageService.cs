namespace DriverLogisticsApp.Services
{
    public class MauiSecureStorageService : ISecureStorageService
    {
        public Task<string> GetAsync(string key)
        {
            return SecureStorage.Default.GetAsync(key);
        }

        public Task SetAsync(string key, string value)
        {
            return SecureStorage.Default.SetAsync(key, value);
        }

        public bool Remove(string key)
        {
            return SecureStorage.Default.Remove(key);
        }
    }
}