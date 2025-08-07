using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class LoginViewModelTests
    {
        private Mock<IAlertService> _mockAlertService;
        private Mock<ISecureStorageService> _mockSecureStorageService;
        private Mock<INavigationService> _mockNavigationService;
        private LoginViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAlertService = new Mock<IAlertService>();
            _mockSecureStorageService = new Mock<ISecureStorageService>();
            _mockNavigationService = new Mock<INavigationService>();
            _viewModel = new LoginViewModel(
                _mockAlertService.Object,
                _mockSecureStorageService.Object,
                _mockNavigationService.Object);
        }

        [TestMethod]
        public async Task LoginCommand_WithCorrectPin_NavigatesBack()
        {
            // ARRANGE
            var savedPin = "1234";
            _viewModel.Pin = "1234";
            _mockSecureStorageService.Setup(s => s.GetAsync("user_pin")).ReturnsAsync(savedPin);

            // ACT
            await _viewModel.LoginCommand.ExecuteAsync(null);

            // ASSERT
            _mockNavigationService.Verify(nav => nav.GoBackAsync(), Times.Once);
        }

        [TestMethod]
        public async Task LoginCommand_WithIncorrectPin_ShowsAlertAndDoesNotNavigate()
        {
            // ARRANGE
            var savedPin = "1234";
            _viewModel.Pin = "9999";
            _mockSecureStorageService.Setup(s => s.GetAsync("user_pin")).ReturnsAsync(savedPin);

            // ACT
            await _viewModel.LoginCommand.ExecuteAsync(null);

            // ASSERT
            _mockAlertService.Verify(s => s.DisplayAlert("Error", "Incorrect PIN. Please try again.", "OK"), Times.Once);
            _mockNavigationService.Verify(nav => nav.GoBackAsync(), Times.Never);
        }
    }
}
