using LiveChartsCore.SkiaSharpView;
using SensorsApp.ViewModels;
namespace SensorsApp.Pages;

public partial class GyroscopePage : ContentPage
{
	public GyroscopePage(GyroscopePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

        int currentLength = (BindingContext as GyroscopePageViewModel).LineSeriesXPoint.Count;
        Axis xAxis = new()
        {
            MaxLimit = currentLength,
            MinLimit = Math.Max(0, currentLength - 10),

        };

        Chart.XAxes = new List<Axis> { xAxis };

        Chart.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.Both; // Zooming in both directions
    }

    private void ResetGraph_Clicked(object sender, EventArgs e)
    {
        int currentLength = (BindingContext as GyroscopePageViewModel).LineSeriesXPoint.Count;
        Axis xAxis = new()
        {
            MaxLimit = currentLength,
            MinLimit = Math.Max(0, currentLength - 10),

        };

        Chart.XAxes = new List<Axis> { xAxis };
        Chart.YAxes = new List<Axis> { new Axis() };
    }
}