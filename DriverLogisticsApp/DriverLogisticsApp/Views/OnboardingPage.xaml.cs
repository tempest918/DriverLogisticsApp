using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace DriverLogisticsApp.Views
{
    public partial class OnboardingPage : ContentPage
    {
        private readonly TaskCompletionSource<string> _tcs;
        private bool _isActionTaken = false;

        public OnboardingPage(string title, string description, bool isFirstStep, bool isLastStep, TaskCompletionSource<string> tcs)
        {
            InitializeComponent();

            _tcs = tcs;

            TitleLabel.Text = title;
            DescriptionLabel.Text = description;

            PreviousButton.IsEnabled = !isFirstStep;

            if (isLastStep)
            {
                NextButton.IsVisible = false;
                DoneButton.IsVisible = true;
            }
            else
            {
                NextButton.IsVisible = true;
                DoneButton.IsVisible = false;
            }
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            _isActionTaken = true;
            _tcs.TrySetResult("next");
            await Navigation.PopModalAsync(false);
        }

        private async void PreviousButton_Clicked(object sender, EventArgs e)
        {
            _isActionTaken = true;
            _tcs.TrySetResult("previous");
            await Navigation.PopModalAsync(false);
        }

        private async void SkipButton_Clicked(object sender, EventArgs e)
        {
            _isActionTaken = true;
            _tcs.TrySetResult("skip");
            await Navigation.PopModalAsync(false);
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            _isActionTaken = true;
            _tcs.TrySetResult("done");
            await Navigation.PopModalAsync(false);
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent the user from dismissing the page with the hardware back button
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Ensure the task is completed if the page is dismissed by other means
            if (!_isActionTaken)
            {
                _tcs.TrySetResult("dismissed");
            }
        }
    }
}
