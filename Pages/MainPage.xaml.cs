using SensorsApp.ViewModels;

namespace SensorsApp.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel; // the connection between the view and the view model
    }
}