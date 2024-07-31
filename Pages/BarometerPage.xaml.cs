using SensorsApp.ViewModels;

namespace SensorsApp.Pages;

public partial class BarometerPage : ContentPage
{
	public BarometerPage(BarometerPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}