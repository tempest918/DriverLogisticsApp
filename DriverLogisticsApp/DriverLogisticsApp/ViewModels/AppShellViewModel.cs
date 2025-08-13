using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.Popups;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace DriverLogisticsApp.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly List<OnboardingStep> _onboardingSteps;

        public AppShellViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _onboardingSteps = new List<OnboardingStep>
            {
                new OnboardingStep { Title = "Welcome to Truck Loads!", Description = "This short tour will walk you through the key features of the app.", Route = $"//{nameof(Views.MainPage)}" },
                new OnboardingStep { Title = "Manage Your Loads", Description = "Create, update, and track your loads from planned to completed.", Route = nameof(Views.AddLoadPage) },
                new OnboardingStep { Title = "Track Your Expenses", Description = "Log expenses for each load, including fuel, tolls, and maintenance.", Route = nameof(Views.AddExpensePage) },
                new OnboardingStep { Title = "Generate Reports", Description = "Create professional PDF invoices and settlement reports.", Route = nameof(Views.SettlementReportPage) },
                new OnboardingStep { Title = "Manage Your Profile", Description = "Use the profile page to set your information and secure the app with a PIN.", Route = nameof(Views.ProfilePage) }
            };

            // Using Task.Run to avoid blocking the constructor
            Task.Run(StartOnboardingIfNeeded);
        }

        private async Task StartOnboardingIfNeeded()
        {
            // A small delay to ensure the initial page has loaded before we navigate
            await Task.Delay(500);

            if (Preferences.Get("OnboardingComplete", false))
            {
                return;
            }

            var currentStep = 0;
            while (currentStep >= 0 && currentStep < _onboardingSteps.Count)
            {
                var step = _onboardingSteps[currentStep];

                // Navigate to the correct page for the step
                if (!string.IsNullOrEmpty(step.Route))
                {
                    await _navigationService.NavigateToAsync(step.Route);
                }

                var isFirstStep = currentStep == 0;
                var isLastStep = currentStep == _onboardingSteps.Count - 1;

                var tcs = new TaskCompletionSource<string>();
                var popup = new OnboardingPopup(step.Title, step.Description, isFirstStep, isLastStep, tcs);

                var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup);
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
                        currentStep = _onboardingSteps.Count; // Exit loop
                        break;
                    default: // "dismissed"
                        currentStep = -1; // Exit loop, but don't mark as complete
                        break;
                }
            }

            if (currentStep >= _onboardingSteps.Count)
            {
                Preferences.Set("OnboardingComplete", true);
                // Navigate back to the main page after onboarding is finished
                await _navigationService.NavigateToAsync($"//{nameof(Views.MainPage)}");
            }
        }
    }
}
