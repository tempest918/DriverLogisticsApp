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
            if (Application.Current?.MainPage != null)
            {
                return Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }

            return Task.CompletedTask;
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

        /// <summary>
        /// request an action sheet from the user with a title, cancel, destruction and buttons options
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cancel"></param>
        /// <param name="destruction"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return Shell.Current.DisplayActionSheet(title, cancel, destruction, buttons);
        }
    }
}