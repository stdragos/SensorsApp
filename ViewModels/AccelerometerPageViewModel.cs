using CommunityToolkit.Mvvm.ComponentModel;
namespace SensorsApp.ViewModels;

using Android.Widget;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using OxyPlot;
using SensorsApp.Models;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

public partial class AccelerometerPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ISeries[] series;

    public ObservableCollection<double> LineSeriesXPoint;
    public ObservableCollection<double> LineSeriesYPoint;
    public ObservableCollection<double> LineSeriesZPoint;

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "Accelerometer log",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };

    public Axis xAxis = new Axis
    {
        MaxLimit = 10,
        MinLimit = 0
    };

    [ObservableProperty]
    private MainPageViewModel mainPageViewModel;

    [ObservableProperty]
    private string accelerometerValues;

    public AccelerometerPageViewModel(MainPageViewModel mainPageViewModel)
    {
        MainPageViewModel = mainPageViewModel;

        LineSeriesXPoint = new ObservableCollection<double>();
        LineSeriesYPoint = new ObservableCollection<double>();
        LineSeriesZPoint = new ObservableCollection<double>();

        var accelerometerData = ReadLinesFromFileAsync(MainPageViewModel.AccelerometerDataPath).Result;
        foreach(var dataPoint in accelerometerData)
        {
            LineSeriesXPoint.Add(dataPoint.X);
            LineSeriesYPoint.Add(dataPoint.Y);
            LineSeriesZPoint.Add(dataPoint.Z);
        }

        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = LineSeriesXPoint,
                Name = "X",
                Fill = null
            },
            new LineSeries<double>
            {
                Values = LineSeriesYPoint,
                Name = "Y",
                Fill = null
            },
            new LineSeries<double>
            {
                Values = LineSeriesZPoint,
                Name = "Z",
                Fill = null
            }
        };

        mainPageViewModel.SensorValuesChanged += AccelerometerChanged;
    }

    private async Task<List<AccelerometerDataPoint>> ReadLinesFromFileAsync(string filePath)
    {
        var lines = new List<string>();
        var dataPoints = new List<AccelerometerDataPoint>();

        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    lines.Add(line);
                }
            }

            dataPoints = lines/*.TakeLast(10)*/
                             .Select(line => ParseLineToDataPoint(line))
                             .ToList();

            return dataPoints;
        }
        else
        {
            throw new FileNotFoundException($"File '{filePath}' does not exist.");
        }
    }

    private AccelerometerDataPoint ParseLineToDataPoint(string line)
    {
        var parts = line.Split(' ');

        if (parts.Length == 3 &&
            double.TryParse(parts[0], out double x) &&
            double.TryParse(parts[1], out double y) &&
            double.TryParse(parts[2], out double z))
        {
            return new AccelerometerDataPoint { X = x, Y = y, Z = z };
        }

        throw new FormatException($"Line '{line}' does not have the expected format.");
    }

    void AccelerometerChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "AccelerometerValues":
                UpdateGraph(MainPageViewModel.AccelerometerValues);
                break;
        }
    }

    void UpdateGraph(string newValues)
    {
        var accelerometerData = ParseLineToDataPoint(newValues);
        LineSeriesXPoint.Add(accelerometerData.X);
        LineSeriesYPoint.Add(accelerometerData.Y);
        LineSeriesZPoint.Add(accelerometerData.Z);
        Console.WriteLine($"Added new data point: {accelerometerData.X} {accelerometerData.Y} {accelerometerData.Z}");
    }
}