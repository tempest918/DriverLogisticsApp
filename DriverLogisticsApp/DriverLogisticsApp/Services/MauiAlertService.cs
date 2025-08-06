namespace DriverLogisticsApp.Services
{
    public class MauiAlertService : IAlertService
    {
        /// <summary>
        /// request a confirmation from the user with a title and message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public Task DisplayAlert(string title, string message, string cancel)
        {
            return Shell.Current.DisplayAlert(title, message, cancel);
        }

        /// <summary>
        /// request a confirmation from the user with a title, message, accept and cancel options
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="accept"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return Shell.Current.DisplayAlert(title, message, accept, cancel);
        }
    }
}