using System;
using Microsoft.Maui.Controls;

namespace DriverLogisticsApp.Views
{
    public partial class OnboardingPage : ContentPage
    {
        public string UserAction { get; private set; }

        public OnboardingPage(string title, string description, bool isFirstStep, bool isLastStep)
        {
            InitializeComponent();

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
            UserAction = "next";
            await Navigation.PopModalAsync(false);
        }

        private async void PreviousButton_Clicked(object sender, EventArgs e)
        {
            UserAction = "previous";
            await Navigation.PopModalAsync(false);
        }

        private async void SkipButton_Clicked(object sender, EventArgs e)
        {
            UserAction = "skip";
            await Navigation.PopModalAsync(false);
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            UserAction = "done";
            await Navigation.PopModalAsync(false);
        }
    }
}
