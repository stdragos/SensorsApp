using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsApp.ViewModels
{
    public partial class BarometerPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private MainPageViewModel mainPageViewModel;

        public BarometerPageViewModel(MainPageViewModel mainPageViewModel)
        {
            MainPageViewModel = mainPageViewModel;
        }
    }
}
