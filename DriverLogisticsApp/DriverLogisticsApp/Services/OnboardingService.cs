using DriverLogisticsApp.Popups;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;

namespace DriverLogisticsApp.Services
{
    public class OnboardingService : IOnboardingService
    {
        private int _currentStep = 0;
        private readonly List<(string Title, string Description)> _steps = new List<(string, string)>
        {
            ("Welcome to Truck Loads!", "This short tour will walk you through the key features of the app."),
            ("Manage Your Loads", "Create, update, and track your loads from planned to completed."),
            ("Track Your Expenses", "Log expenses for each load, including fuel, tolls, and maintenance. You can even attach receipt photos!"),
            ("Generate Reports", "Create professional PDF invoices and settlement reports."),
            ("Secure Your Data", "Use the PIN lock feature to keep your financial data safe."),
            ("Get Started!", "You're all set! Tap 'Done' to start using the app.")
        };

        public void StartOnboarding()
        {
            if (Preferences.Get("OnboardingComplete", false))
            {
                return;
            }

            ShowStep(_currentStep);
        }

        private void ShowStep(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= _steps.Count)
            {
                return;
            }

            var step = _steps[stepIndex];
            var popup = new OnboardingPopup(step.Title, step.Description,
                onNext: () =>
                {
                    _currentStep++;
                    if (_currentStep < _steps.Count)
                    {
                        ShowStep(_currentStep);
                    }
                    else
                    {
                        Preferences.Set("OnboardingComplete", true);
                    }
                },
                onPrevious: () =>
                {
                    _currentStep--;
                    ShowStep(_currentStep);
                },
                onSkip: () =>
                {
                    Preferences.Set("OnboardingComplete", true);
                });

            var previousButton = popup.FindByName<Button>("PreviousButton");
            if (previousButton != null)
            {
                previousButton.IsEnabled = stepIndex > 0;
            }

            if (stepIndex == _steps.Count - 1)
            {
                var nextButton = popup.FindByName<Button>("NextButton");
                if (nextButton != null)
                {
                    nextButton.IsVisible = false;
                }

                var doneButton = popup.FindByName<Button>("DoneButton");
                if (doneButton != null)
                {
                    doneButton.IsVisible = true;
                }
            }

            Shell.Current.CurrentPage.ShowPopup(popup);
        }
    }
}
