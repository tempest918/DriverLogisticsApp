using CommunityToolkit.Maui.Views;

namespace DriverLogisticsApp.Popups
{
    /// <summary>
    /// initialize a popup to display a zoomed image
    /// </summary>
    public partial class ZoomedImagePopup : Popup
    {
        public static readonly BindableProperty ImagePathProperty =
            BindableProperty.Create(nameof(ImagePath), typeof(string), typeof(ZoomedImagePopup));

        /// <summary>
        /// the path to the image to be displayed in the popup
        /// </summary>
        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        /// <summary>
        /// the constructor for the ZoomedImagePopup
        /// </summary>
        /// <param name="imagePath"></param>
        public ZoomedImagePopup(string imagePath)
        {
            InitializeComponent();
            ImagePath = imagePath;
            BindingContext = this;

            var displayInfo = DeviceDisplay.MainDisplayInfo;
            var screenWidth = displayInfo.Width / displayInfo.Density;
            var screenHeight = displayInfo.Height / displayInfo.Density;

            PopupContainer.WidthRequest = screenWidth * 0.9;
            PopupContainer.HeightRequest = screenHeight * 0.9;
        }
    }
}