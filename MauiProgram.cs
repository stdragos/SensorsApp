using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SensorsApp.Pages;
using SensorsApp.ViewModels;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace SensorsApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp(true)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddTransient<AccelerometerPage>();
            builder.Services.AddTransient<AccelerometerPageViewModel>();

            builder.Services.AddTransient<GyroscopePage>();
            builder.Services.AddTransient<GyroscopePageViewModel>();

            builder.Services.AddTransient<BarometerPage>();
            builder.Services.AddTransient<BarometerPageViewModel>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
