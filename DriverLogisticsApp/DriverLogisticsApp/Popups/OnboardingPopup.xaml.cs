using CommunityToolkit.Maui.Views;
using System;

namespace DriverLogisticsApp.Popups
{
    public partial class OnboardingPopup : Popup
    {
        private readonly Action _onNext;
        private readonly Action _onPrevious;
        private readonly Action _onSkip;

        public OnboardingPopup(string title, string description, bool isFirstStep, bool isLastStep, Action onNext, Action onPrevious, Action onSkip)
        {
            InitializeComponent();

            TitleLabel.Text = title;
            DescriptionLabel.Text = description;

            _onNext = onNext;
            _onPrevious = onPrevious;
            _onSkip = onSkip;

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
            _onNext?.Invoke();
            (this as CommunityToolkit.Maui.Views.Popup)?.CloseAsync();
        }

        private void PreviousButton_Clicked(object sender, EventArgs e)
        {
            _onPrevious?.Invoke();
            (this as CommunityToolkit.Maui.Views.Popup)?.CloseAsync();
        }

        private void SkipButton_Clicked(object sender, EventArgs e)
        {
            _onSkip?.Invoke();
            (this as CommunityToolkit.Maui.Views.Popup)?.CloseAsync();
        }

        private void DoneButton_Clicked(object sender, EventArgs e)
        {
            _onNext?.Invoke();
            (this as CommunityToolkit.Maui.Views.Popup)?.CloseAsync();
        }
    }
}
