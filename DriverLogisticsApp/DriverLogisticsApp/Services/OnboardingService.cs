using DriverLogisticsApp.Views;
using DriverLogisticsApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace DriverLogisticsApp.Services
{
    public class OnboardingService : IOnboardingService
    {
        private const string OnboardingStepKey = "OnboardingStep";
        private readonly List<OnboardingStep> _steps = new List<OnboardingStep>
        {
            new OnboardingStep { Title = "Welcome to Truck Loads!", Description = "This short tour will walk you through the key features of the app." },
            new OnboardingStep { Title = "Manage Your Loads", Description = "Create, update, and track your loads from planned to completed." },
            new OnboardingStep { Title = "Track Your Expenses", Description = "Log expenses for each load, including fuel, tolls, and maintenance. You can even attach receipt photos!" },
            new OnboardingStep { Title = "Generate Reports", Description = "Create professional PDF invoices and settlement reports." },
            new OnboardingStep { Title = "Secure Your Data", Description = "Use the PIN lock feature to keep your financial data safe." },
            new OnboardingStep { Title = "Get Started!", Description = "You're all set! Tap 'Done' to start using the app." }
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

            // Get the current step from preferences, default to 0 if not set
            var currentStep = Preferences.Get(OnboardingStepKey, 0);

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
                        currentStep = _steps.Count; // Set to exit loop
                        break;
                    default: // "dismissed" or other unknown action
                        // Stop the onboarding flow, but don't mark it as complete
                        Preferences.Remove(OnboardingStepKey);
                        return;
                }

                // Save the new step to preferences
                Preferences.Set(OnboardingStepKey, currentStep);
            }

            // Onboarding finished, set complete and clear the step key
            Preferences.Set("OnboardingComplete", true);
            Preferences.Remove(OnboardingStepKey);
        }
    }
}
