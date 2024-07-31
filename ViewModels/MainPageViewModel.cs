namespace SensorsApp.ViewModels;
using CommunityToolkit.Maui.Alerts;

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.PlatformConfiguration;
using SensorsApp.Pages;
using System.ComponentModel;
using System.Diagnostics;
using Android.App;
using Android.OS;

public partial class MainPageViewModel : ObservableObject
{

    private string _docsDirectory;
    public string AccelerometerDataPath { get; set; }
    public string GyroscopeDataPath { get; set; }

 


    private DateTime _lastReadingTime;
    private TimeSpan _readingInterval = TimeSpan.FromSeconds(2);

    public MainPageViewModel()
	{
        _docsDirectory = Application.Context.GetExternalFilesDir(Environment.DirectoryDocuments).ToString();

        if(_docsDirectory is null)
        {
            throw new FileNotFoundException($"Path '{_docsDirectory}' does not exist.");
        }

        AccelerometerDataPath = Path.Combine(_docsDirectory, "accelerometerData.txt");
        GyroscopeDataPath = Path.Combine(_docsDirectory, "gyroscopeData.txt");
        if(!File.Exists(AccelerometerDataPath))
        {
            File.Create(AccelerometerDataPath);
        }
        else
        {
            File.Delete(AccelerometerDataPath);
        }
        if(!File.Exists(GyroscopeDataPath))
        {
            File.Create(GyroscopeDataPath);
        }
        else
        {
           File.Delete(GyroscopeDataPath);
        }

        LaunchSensors();
    }

    [RelayCommand]
    public async Task NavigateToAccelerometerPage()
    {
        await Shell.Current.GoToAsync(nameof(AccelerometerPage));
    }

    [RelayCommand]
    public async Task NavigateToGyroscopePage()
    {
        await Shell.Current.GoToAsync(nameof(GyroscopePage));
    }  
    
    [RelayCommand]
    public async Task NavigateToBarometerPage()
    {
        await Shell.Current.GoToAsync(nameof(BarometerPage));
    }

    private void LaunchSensors()
    {
        new Thread(() =>
        {
            LaunchAccelerometer();
        }).Start();

        new Thread(() =>
        {
            LaunchGyroscope();
        }).Start();

        new Thread(() =>
        {
            LaunchBarometer();
        }).Start();
    }


    #region accelerometer
    private void LaunchAccelerometer()
    {
        if (Accelerometer.Default.IsSupported)
        {
            if (!Accelerometer.Default.IsMonitoring)
            {
                // Turn on accelerometer
                Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                Accelerometer.Default.Start(SensorSpeed.Game);
            }
            else
            {
                // Turn off accelerometer
                Accelerometer.Default.Stop();
                Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
            }
        }
    }

    [ObservableProperty]
    private string accelerometerValues;

    private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        var currentTime = DateTime.Now;
        if (currentTime - _lastReadingTime > _readingInterval)
        {
            AccelerometerValues = $"{e.Reading.Acceleration.X} {e.Reading.Acceleration.Y} {e.Reading.Acceleration.Z}";
            NotifySensorValuesChanged(nameof(AccelerometerValues));
            File.AppendAllTextAsync(AccelerometerDataPath, $"{e.Reading.Acceleration.X} {e.Reading.Acceleration.Y} {e.Reading.Acceleration.Z}" + '\n');
            _lastReadingTime = currentTime;
        }
    }

    #endregion

    #region gyroscope
    private void LaunchGyroscope()
    {
        if (Gyroscope.Default.IsSupported)
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                // Turn on gyroscope
                Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
                Gyroscope.Default.Start(SensorSpeed.Game);
            }
            else
            {
                // Turn off gyroscope
                Gyroscope.Default.Stop();
                Gyroscope.Default.ReadingChanged -= Gyroscope_ReadingChanged;
            }
        }
    }

    [ObservableProperty]
    private string gyroscopeValues;

    private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
        var currentTime = DateTime.Now;
        if (currentTime - _lastReadingTime > _readingInterval)
        {
            GyroscopeValues = $"{e.Reading}";
            NotifySensorValuesChanged(nameof(GyroscopeValues));
            File.AppendAllTextAsync(GyroscopeDataPath, $"{e.Reading}" + '\n');
            _lastReadingTime = currentTime;
        }
    }


    #endregion

    #region barometer

    public void LaunchBarometer()
    {
        if (Barometer.Default.IsSupported)
        {
            if (!Barometer.Default.IsMonitoring)
            {
                // Turn on barometer
                Barometer.Default.ReadingChanged += Barometer_ReadingChanged;
                Barometer.Default.Start(SensorSpeed.UI);
            }
            else
            {
                // Turn off barometer
                Barometer.Default.Stop();
                Barometer.Default.ReadingChanged -= Barometer_ReadingChanged;
            }
        }
    }

    [ObservableProperty]
    private string barometerValues;

    private void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
    {
        var currentTime = DateTime.Now;
        if (currentTime - _lastReadingTime > _readingInterval)
        {
            BarometerValues = $"{e.Reading}";
            _lastReadingTime = currentTime;
        }
    }

    #endregion

    #region sensorschanged

    public event PropertyChangedEventHandler SensorValuesChanged;
    private readonly object _lock = new object();
    private void NotifySensorValuesChanged(string sensorName)
    {
        lock (_lock)
        {
            SensorValuesChanged?.Invoke(this, new PropertyChangedEventArgs(sensorName));
        }
    }

    #endregion
}