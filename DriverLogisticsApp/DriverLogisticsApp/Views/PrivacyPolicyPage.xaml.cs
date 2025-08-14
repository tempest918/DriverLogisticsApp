namespace DriverLogisticsApp.Views;

public partial class PrivacyPolicyPage : ContentPage
{
	public PrivacyPolicyPage()
	{
		InitializeComponent();
        LoadPrivacyPolicy();
    }

    private async void LoadPrivacyPolicy()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("privacypolicy.html");
            using var reader = new StreamReader(stream);
            var htmlContent = await reader.ReadToEndAsync();
            webView.Source = new HtmlWebViewSource { Html = htmlContent };
        }
        catch (Exception ex)
        {
            // Handle exceptions, e.g., file not found
            await DisplayAlert("Error", "Could not load the privacy policy.", "OK");
        }
    }

    private async void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await Launcher.OpenAsync(new Uri(e.Url));
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not open email client.", "OK");
            }
        }
    }
}
