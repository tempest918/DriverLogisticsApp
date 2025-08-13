using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DriverLogisticsApp.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly List<OnboardingStep> _onboardingSteps;
        private int _currentOnboardingStep;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsOnboardingFirstStep))]
        [NotifyPropertyChangedFor(nameof(IsOnboardingLastStep))]
        private bool _isOnboardingVisible;

        [ObservableProperty]
        private string _onboardingTitle;

        [ObservableProperty]
        private string _onboardingDescription;

        public bool IsOnboardingFirstStep => _currentOnboardingStep == 0;
        public bool IsOnboardingLastStep => _currentOnboardingStep == _onboardingSteps.Count - 1;

        public IAsyncRelayCommand NextOnboardingStepCommand { get; }
        public IAsyncRelayCommand PreviousOnboardingStepCommand { get; }
        public IAsyncRelayCommand SkipOnboardingCommand { get; }

        public AppShellViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NextOnboardingStepCommand = new AsyncRelayCommand(NextOnboardingStep);
            PreviousOnboardingStepCommand = new AsyncRelayCommand(PreviousOnboardingStep);
            SkipOnboardingCommand = new AsyncRelayCommand(SkipOnboarding);

            _onboardingSteps = new List<OnboardingStep>
            {
                new OnboardingStep { Title = "Welcome to Truck Loads!", Description = "This short tour will walk you through the key features of the app.", Route = $"//{nameof(Views.MainPage)}" },
                new OnboardingStep { Title = "Manage Your Loads", Description = "Create, update, and track your loads from planned to completed.", Route = nameof(Views.AddLoadPage) },
                new OnboardingStep { Title = "Track Your Expenses", Description = "Log expenses for each load, including fuel, tolls, and maintenance.", Route = nameof(Views.AddExpensePage) },
                new OnboardingStep { Title = "Generate Reports", Description = "Create professional PDF invoices and settlement reports.", Route = nameof(Views.SettlementReportPage) },
                new OnboardingStep { Title = "Manage Your Profile", Description = "Use the profile page to set your information and secure the app with a PIN.", Route = nameof(Views.ProfilePage) }
            };

            StartOnboarding();
        }

        private void StartOnboarding()
        {
            if (Preferences.Get("OnboardingComplete", false))
            {
                return;
            }
            _currentOnboardingStep = 0;
            UpdateOnboardingStep();
            IsOnboardingVisible = true;
        }

        private async Task UpdateOnboardingStep()
        {
            var step = _onboardingSteps[_currentOnboardingStep];
            OnboardingTitle = step.Title;
            OnboardingDescription = step.Description;

            if (!string.IsNullOrEmpty(step.Route))
            {
                await _navigationService.NavigateToAsync(step.Route);
            }

            OnPropertyChanged(nameof(IsOnboardingFirstStep));
            OnPropertyChanged(nameof(IsOnboardingLastStep));
        }

        private async Task NextOnboardingStep()
        {
            if (_currentOnboardingStep < _onboardingSteps.Count - 1)
            {
                _currentOnboardingStep++;
                await UpdateOnboardingStep();
            }
            else
            {
                await SkipOnboarding(); // Finish on the last step
            }
        }

        private async Task PreviousOnboardingStep()
        {
            if (_currentOnboardingStep > 0)
            {
                _currentOnboardingStep--;
                await UpdateOnboardingStep();
            }
        }

        private async Task SkipOnboarding()
        {
            IsOnboardingVisible = false;
            Preferences.Set("OnboardingComplete", true);
            // Navigate back to the main page after onboarding is finished
            await _navigationService.NavigateToAsync($"//{nameof(Views.MainPage)}");
        }
    }
}
