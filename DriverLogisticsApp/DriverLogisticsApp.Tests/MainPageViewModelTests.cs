using DriverLogisticsApp.Models;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class MainPageViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<INavigationService> _mockNavigationService;
        private MainPageViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockNavigationService = new Mock<INavigationService>();
            _viewModel = new MainPageViewModel(
                _mockDbService.Object,
                _mockNavigationService.Object);
        }

        /// <summary>
        /// tests that the GoToDetailsCommand navigates to the LoadDetailsPage
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GoToDetailsCommand_Navigates_WithCorrectLoadId()
        {
            // ARRANGE
            var testLoad = new Load { Id = 789 };

            // ACT
            await _viewModel.GoToDetailsCommand.ExecuteAsync(testLoad);

            // ASSERT
            _mockNavigationService.Verify(nav => nav.NavigateToAsync(
                nameof(Views.LoadDetailsPage),
                It.Is<IDictionary<string, object>>(d => (int)d["LoadId"] == 789)),
                Times.Once);
        }

        /// <summary>
        /// tests that setting the SearchText property filters the Loads collection correctly
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SearchText_FiltersLoads_Correctly()
        {
            // ARRANGE
            var allLoads = new List<Load>
            {
                new Load { Id = 1, LoadNumber = "ABC", ShipperName = "Apple" },
                new Load { Id = 2, LoadNumber = "DEF", ShipperName = "Banana" },
                new Load { Id = 3, LoadNumber = "GHI", ShipperName = "Apple Inc" }
            };
            _mockDbService.Setup(db => db.GetLoadsAsync()).ReturnsAsync(allLoads);

            // ACT
            await _viewModel.GetLoadsCommand.ExecuteAsync(null);
            _viewModel.SearchText = "apple";

            // ASSERT
            Assert.AreEqual(2, _viewModel.Loads.Count);
            Assert.IsTrue(_viewModel.Loads.Any(l => l.ShipperName == "Apple"));
            Assert.IsTrue(_viewModel.Loads.Any(l => l.ShipperName == "Apple Inc"));
        }
    }
}
