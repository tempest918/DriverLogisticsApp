using DriverLogisticsApp.Models;
using DriverLogisticsApp.Models.ExpenseTypes;
using DriverLogisticsApp.Services;
using DriverLogisticsApp.ViewModels;
using Moq;
using IPreferences = DriverLogisticsApp.Services.IPreferences;

namespace DriverLogisticsApp.Tests
{
    [TestClass]
    public class MainPageViewModelTests
    {
        private Mock<IDatabaseService> _mockDbService;
        private Mock<INavigationService> _mockNavigationService;
        private Mock<IPreferences> _mockPreferencesService;
        private MainPageViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDbService = new Mock<IDatabaseService>();
            _mockNavigationService = new Mock<INavigationService>();
            _mockPreferencesService = new Mock<IPreferences>();
            _viewModel = new MainPageViewModel(
                _mockDbService.Object,
                _mockNavigationService.Object,
                _mockPreferencesService.Object
                );
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
                new Load { Id = 1, LoadNumber = "ABC", ShipperName = "Apple", Status = "Planned" },
                new Load { Id = 2, LoadNumber = "DEF", ShipperName = "Banana", Status = "Planned" },
                new Load { Id = 3, LoadNumber = "GHI", ShipperName = "Apple Inc", Status = "Planned" }
            };
            _mockDbService.Setup(db => db.GetLoadsAsync()).ReturnsAsync(allLoads);
            _mockDbService.Setup(db => db.GetExpensesForLoadAsync(0)).ReturnsAsync(new List<Models.ExpenseTypes.Expense>());


            // ACT
            await _viewModel.GetLoadsCommand.ExecuteAsync(null);
            _viewModel.SearchText = "apple";

            // ASSERT
            Assert.AreEqual(2, _viewModel.Loads.Count);
            Assert.IsTrue(_viewModel.Loads.Any(l => l.ShipperName == "Apple"));
            Assert.IsTrue(_viewModel.Loads.Any(l => l.ShipperName == "Apple Inc"));
        }

        /// <summary>
        /// tests that CalculateKpisAsync calculates the revenue, expenses, and net profit correctly
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CalculateKpisAsync_CalculatesRevenueCorrectly()
        {
            // ARRANGE
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var allLoads = new List<Load>
            {
                new Load { Status = "Completed", FreightRate = 1000, DeliveryDate = startOfMonth },
                new Load { Status = "Invoiced", FreightRate = 1500, DeliveryDate = startOfMonth },
                new Load { Status = "In Progress", FreightRate = 2000, DeliveryDate = startOfMonth },
                new Load { Status = "Planned", FreightRate = 500, DeliveryDate = startOfMonth },
                new Load { Status = "Cancelled", FreightRate = 100, DeliveryDate = startOfMonth },

                new Load { Status = "Completed", FreightRate = 999, DeliveryDate = startOfMonth.AddMonths(-1) }
            };

            var allExpenses = new List<Models.ExpenseTypes.Expense>
            {
                new FuelExpense { Amount = 100, Date = startOfMonth },
                new FuelExpense { Amount = 50, Date = startOfMonth.AddMonths(-1) }
            };

            _mockDbService.Setup(db => db.GetLoadsAsync()).ReturnsAsync(allLoads);
            _mockDbService.Setup(db => db.GetExpensesForLoadAsync(0)).ReturnsAsync(allExpenses);

            // ACT
            await _viewModel.GetLoadsCommand.ExecuteAsync(null);

            // ASSERT
            Assert.AreEqual(2500m, _viewModel.ActualRevenue);
            Assert.AreEqual(2500m, _viewModel.PotentialRevenue);
            Assert.AreEqual(100m, _viewModel.TotalExpenses);
            Assert.AreEqual(2400m, _viewModel.NetProfit);
        }
    }
}
