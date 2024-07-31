using SensorsApp.Pages;

namespace SensorsApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AccelerometerPage), typeof(AccelerometerPage));
            Routing.RegisterRoute(nameof(GyroscopePage), typeof(GyroscopePage));
            Routing.RegisterRoute(nameof(BarometerPage), typeof(BarometerPage));
        }
    }
}
