using CommunityToolkit.Maui.Views;
using System;

namespace DriverLogisticsApp.Popups
{
    public partial class OnboardingPopup : Popup
    {
        public OnboardingPopup(string title, string description, bool isFirstStep, bool isLastStep)
        {
            InitializeComponent();

            TitleLabel.Text = title;
            DescriptionLabel.Text = description;

            var previousButton = this.FindByName<Button>("PreviousButton");
            if (previousButton != null)
            {
                previousButton.IsEnabled = !isFirstStep;
            }

            var nextButton = this.FindByName<Button>("NextButton");
            var doneButton = this.FindByName<Button>("DoneButton");

            if (nextButton != null && doneButton != null)
            {
                if (isLastStep)
                {
                    nextButton.IsVisible = false;
                    doneButton.IsVisible = true;
                }
                else
                {
                    nextButton.IsVisible = true;
                    doneButton.IsVisible = false;
                }
            }
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            this.Close("next");
        }

        private void PreviousButton_Clicked(object sender, EventArgs e)
        {
            this.Close("previous");
        }

        private void SkipButton_Clicked(object sender, EventArgs e)
        {
            this.Close("skip");
        }

        private void DoneButton_Clicked(object sender, EventArgs e)
        {
            this.Close("done");
        }
    }
}
