using DriverLogisticsApp.Popups;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
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

        public async Task StartOnboarding()
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

                var popup = new OnboardingPopup(step.Title, step.Description, isFirstStep, isLastStep);
                var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as string;

                switch (result)
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
                        currentStep = _steps.Count; // Exit loop if popup is dismissed
                        break;
                }
            }

            Preferences.Set("OnboardingComplete", true);
        }
    }
}
