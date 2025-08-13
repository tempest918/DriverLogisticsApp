using DriverLogisticsApp.Views;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace DriverLogisticsApp.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly List<(string Title, string Description)> _steps = new List<(string, string)>
        {
            ("Welcome to Truck Loads!", "This short tour will walk you through the key features of the app."),
            ("Manage Your Loads", "Create, update, and track your loads from planned to completed."),
            ("Track Your Expenses", "Log expenses for each load, including fuel, tolls, and maintenance. You can even attach receipt photos!"),
            ("Generate Reports", "Create professional PDF invoices and settlement reports."),
            ("Secure Your Data", "Use the PIN lock feature to keep your financial data safe."),
            ("Get Started!", "You're all set! Tap 'Done' to start using the app.")
        };

        public OnboardingService()
        {
        }

        public async Task StartOnboardingIfNeeded()
        {
            if (Preferences.Get("OnboardingComplete", false))
            {
                return;
            }

            var currentStep = 0;
            while (currentStep < _steps.Count)
            {
                var step = _steps[currentStep];
                var isFirstStep = currentStep == 0;
                var isLastStep = currentStep == _steps.Count - 1;

                var onboardingPage = new OnboardingPage(step.Title, step.Description, isFirstStep, isLastStep);

                await Shell.Current.Navigation.PushModalAsync(onboardingPage, false);

                var userAction = onboardingPage.UserAction;
                System.Diagnostics.Debug.WriteLine($"[OnboardingService] User action was: {userAction ?? "null"}");

                switch (userAction)
                {
                    case "next":
                        currentStep++;
                        break;
                    case "previous":
                        currentStep--;
                        break;
                    case "skip":
                    case "done":
                        currentStep = _steps.Count; // Exit loop
                        break;
                    default:
                        currentStep = _steps.Count; // Exit loop if page is dismissed
                        break;
                }
            }

            Preferences.Set("OnboardingComplete", true);
        }
    }
}
