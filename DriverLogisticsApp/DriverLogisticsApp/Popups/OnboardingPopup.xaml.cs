using CommunityToolkit.Maui.Views;
using System;

namespace DriverLogisticsApp.Popups
{
    public partial class OnboardingPopup : Popup
    {
        private readonly Action _onNext;
        private readonly Action _onPrevious;
        private readonly Action _onSkip;

        public OnboardingPopup(string title, string description, Action onNext, Action onPrevious, Action onSkip)
        {
            InitializeComponent();

            TitleLabel.Text = title;
            DescriptionLabel.Text = description;

            _onNext = onNext;
            _onPrevious = onPrevious;
            _onSkip = onSkip;
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            _onNext?.Invoke();
            (this as CommunityToolkit.Maui.Views.Popup)?.Close();
        }

        private void PreviousButton_Clicked(object sender, EventArgs e)
        {
            _onPrevious?.Invoke();
            (this as CommunityToolkit.Maui.Views.Popup)?.Close();
        }

        private void SkipButton_Clicked(object sender, EventArgs e)
        {
            _onSkip?.Invoke();
            (this as CommunityToolkit.Maui.Views.Popup)?.Close();
        }

        private void DoneButton_Clicked(object sender, EventArgs e)
        {
            _onNext?.Invoke();
            (this as CommunityToolkit.Maui.Views.Popup)?.Close();
        }
    }
}
