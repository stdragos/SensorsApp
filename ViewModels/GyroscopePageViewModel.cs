using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SensorsApp.Models;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SensorsApp.ViewModels;

public partial class GyroscopePageViewModel : ObservableObject
{
    [ObservableProperty]
    private ISeries[] series;

    public ObservableCollection<double> LineSeriesXPoint;
    public ObservableCollection<double> LineSeriesYPoint;
    public ObservableCollection<double> LineSeriesZPoint;

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "Gyroscope log",
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



	public GyroscopePageViewModel(MainPageViewModel mainPageViewModel)
	{
        MainPageViewModel = mainPageViewModel;

        LineSeriesXPoint = new ObservableCollection<double>();
        LineSeriesYPoint = new ObservableCollection<double>();
        LineSeriesZPoint = new ObservableCollection<double>();

        var gyroscopeData = ReadLinesFromFileAsync(MainPageViewModel.GyroscopeDataPath).Result;

        foreach(var dataPoint in gyroscopeData)
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

        mainPageViewModel.SensorValuesChanged += GyroscopeChanged;

    }

    private async Task<List<GyroscopeDataPoint>> ReadLinesFromFileAsync(string filePath)
    {
        var lines = new List<string>();
        var dataPoints = new List<GyroscopeDataPoint>();

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

    private GyroscopeDataPoint ParseLineToDataPoint(string line)
    {
        var keyValuePairs = line.Split(", ");
        Dictionary<string, string> values = new Dictionary<string, string>();

        foreach (string pair in keyValuePairs)
        {
            // Split each pair by '='
            string[] keyValue = pair.Split(':');
            // Store the value in the dictionary with the corresponding key
            values[keyValue[0]] = keyValue[1];
        }

        if(values.Count == 3)
        {
            return new GyroscopeDataPoint
            {
                X = double.Parse(values["X"]),
                Y = double.Parse(values["Y"]),
                Z = double.Parse(values["Z"])
            };
        }

        throw new FormatException($"Line '{line}' does not have the expected format.");
    }

    void GyroscopeChanged(object sender, PropertyChangedEventArgs e)
    {
        Console.WriteLine("GyroscopeChangeddsads");
        switch (e.PropertyName)
        {
            case "GyroscopeValues":
                UpdateGraph(MainPageViewModel.GyroscopeValues);
                break;
        }
    }

    void UpdateGraph(string newValues)
    {
        var gyroscopeValues = ParseLineToDataPoint(newValues);

        LineSeriesXPoint.Add(gyroscopeValues.X);
        LineSeriesYPoint.Add(gyroscopeValues.Y);
        LineSeriesZPoint.Add(gyroscopeValues.Z);

        Console.WriteLine($"Added new data point: X={gyroscopeValues.X} Y={gyroscopeValues.Y} Z={gyroscopeValues.Z}");
    }
}