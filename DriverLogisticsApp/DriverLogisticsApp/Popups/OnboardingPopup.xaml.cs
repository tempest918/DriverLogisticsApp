using CommunityToolkit.Maui.Views;

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
            Close();
        }

        private void PreviousButton_Clicked(object sender, EventArgs e)
        {
            _onPrevious?.Invoke();
            Close();
        }

        private void SkipButton_Clicked(object sender, EventArgs e)
        {
            _onSkip?.Invoke();
            Close();
        }

        private void DoneButton_Clicked(object sender, EventArgs e)
        {
            _onNext?.Invoke();
            Close();
        }
    }
}
