using CommunityToolkit.Maui.Views;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace DriverLogisticsApp.Popups
{
    public partial class OnboardingPopup : Popup
    {
        private readonly TaskCompletionSource<string> _tcs;

        public OnboardingPopup(string title, string description, bool isFirstStep, bool isLastStep, TaskCompletionSource<string> tcs)
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

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            _tcs.TrySetResult("next");
            this.Close();
        }

        private void PreviousButton_Clicked(object sender, EventArgs e)
        {
            _tcs.TrySetResult("previous");
            this.Close();
        }

        private void SkipButton_Clicked(object sender, EventArgs e)
        {
            _tcs.TrySetResult("skip");
            this.Close();
        }

        private void DoneButton_Clicked(object sender, EventArgs e)
        {
            _tcs.TrySetResult("done");
            this.Close();
        }

        protected override void OnDismissedByTappingOutsideOfPopup()
        {
            _tcs.TrySetResult("dismissed");
            base.OnDismissedByTappingOutsideOfPopup();
        }
    }
}
