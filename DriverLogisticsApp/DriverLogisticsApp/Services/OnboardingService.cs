using DriverLogisticsApp.Views;
using DriverLogisticsApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace DriverLogisticsApp.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly List<OnboardingStep> _steps = new List<OnboardingStep>
        {
            new OnboardingStep { Title = "Welcome to Truck Loads!", Description = "This short tour will walk you through the key features of the app." },
            new OnboardingStep { Title = "Manage Your Loads", Description = "Create, update, and track your loads from planned to completed." },
            new OnboardingStep { Title = "Track Your Expenses", Description = "Log expenses for each load, including fuel, tolls, and maintenance. You can even attach receipt photos!" },
            new OnboardingStep { Title = "Generate Reports", Description = "Create professional PDF invoices and settlement reports." },
            new OnboardingStep { Title = "Secure Your Data", Description = "Use the PIN lock feature to keep your financial data safe." },
            new OnboardingStep { Title = "Get Started!", Description = "You're all set! Tap 'Done' to start using the app." }
        };

        private static bool _onboardingInProgress = false;

        public OnboardingService()
        {
        }

        public async Task StartOnboardingIfNeeded()
        {
            if (Preferences.Get("OnboardingComplete", false) || _onboardingInProgress)
            {
                return;
            }

            _onboardingInProgress = true;
            var currentStep = 0;

            while (currentStep < _steps.Count)
            {
                var step = _steps[currentStep];
                var isFirstStep = currentStep == 0;
                var isLastStep = currentStep == _steps.Count - 1;

                var tcs = new TaskCompletionSource<string>();
                var onboardingPage = new OnboardingPage(step.Title, step.Description, isFirstStep, isLastStep, tcs);

                await Shell.Current.Navigation.PushModalAsync(onboardingPage, false);

                var userAction = await tcs.Task;

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
                    default: // "dismissed"
                        currentStep = _steps.Count; // Exit loop
                        break;
                }
            }

            Preferences.Set("OnboardingComplete", true);
            _onboardingInProgress = false;
        }
    }
}
