using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MouseTracker {
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public const UInt32 SPI_SETMOUSESPEED = 0x0071;
        public const UInt32 SPI_SETWHEELSCROLLLINES = 0x0069;
        public const UInt32 SPI_SETDOUBLECLICKTIME = 0x0020;
        [DllImport("user32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            UInt32 pvParam,
            UInt32 fWinIni);


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)  {
            this.Close();
        }

        private void SelectProfile(object sender, RoutedEventArgs e)  {

        }

        private void SliderFunction(object sender, RoutedPropertyChangedEventArgs<double> e)  {
            uint svalue =  Convert.ToUInt32(Math.Round(SliderSpeed.Value));
            Slidervalue.Content = Math.Round(SliderSpeed.Value);
            SystemParametersInfo(SPI_SETMOUSESPEED, 0, svalue, 0);
        }

        private void SliderFunctionWheel(object sender, RoutedPropertyChangedEventArgs<double> e) {
            uint svaluewheel = Convert.ToUInt32(Math.Round(SliderSpeedWheel.Value));
            Slidervaluewheel.Content = Math.Round(SliderSpeedWheel.Value);
            SystemParametersInfo(SPI_SETWHEELSCROLLLINES, svaluewheel, 0, 0);
        }

        private void SliderFunctionDbc(object sender, RoutedPropertyChangedEventArgs<double> e) {
            uint svaluedbc = Convert.ToUInt32(Math.Round(SliderSpeedDbc.Value));
            SlidervalueDbc.Content = Math.Round(SliderSpeedDbc.Value);
            SystemParametersInfo(SPI_SETWHEELSCROLLLINES, svaluedbc, 0, 0);
        }

        private void Setdfl(object sender, RoutedEventArgs e) {
            Slidervalue.Content = 10;
            SliderSpeed.Value = 10;
            Slidervaluewheel.Content = 3;
            SliderSpeedWheel.Value = 3;
            SlidervalueDbc.Content = 500;
            SliderSpeedDbc.Value = 500;
        }
    }
}
