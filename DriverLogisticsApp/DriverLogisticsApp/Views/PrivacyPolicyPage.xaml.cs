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
}
