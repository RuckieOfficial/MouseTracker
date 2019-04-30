using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using System.Windows.Threading;

namespace MouseTracker {
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        string path = @"../../profiles.json";
        List<Profile> profiles = new List<Profile>();
        List<Profile> WebProfiles = new List<Profile>();

        DispatcherTimer succ = new DispatcherTimer();
        DispatcherTimer newsucc = new DispatcherTimer();

        public const UInt32 SPI_SETMOUSESPEED = 0x0071;
        public const UInt32 SPI_SETWHEELSCROLLLINES = 0x0069;
        public const UInt32 SPI_SETDOUBLECLICKTIME = 0x0020;

        //public const UInt32 SPI_GETMOUSESPEED = 0x0070;
        [DllImport("user32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            UInt32 pvParam,
            UInt32 fWinIni);


        public MainWindow() {
            InitializeComponent();
            Load();
            LoadFromWeb();
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
        
        public async void LoadFromWeb() {

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://ruckelu16.sps-prosek.cz/Home/2018/phpfromsql.php");
            string jsonContent = await response.Content.ReadAsStringAsync();
            WebProfiles = JsonConvert.DeserializeObject<List<Profile>>(jsonContent, settings);

            //var x = WebProfiles.Name;
        }

        private void Load() {
            try {
                string jsonFromFile = File.ReadAllText(path);
                profiles = JsonConvert.DeserializeObject<List<Profile>>(jsonFromFile, settings);
            }
            catch { CreateProfiles(); }
            foreach (Profile profile in profiles) {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = profile.Name;
                item.Selected += new RoutedEventHandler(SelectProfile);
                ProfileBox.Items.Add(item);
            }
        }

        private void CreateProfile(object sender, RoutedEventArgs e) {
            profiles.Add(new Profile(NewProfBox.Text, 10, 3, 500));
            SaveProfiles(profiles);
            ProfileBox.Items.Clear();
            ComboBoxItem choose = new ComboBoxItem();
            choose.Content = "Vyber profil";
            choose.IsSelected = true;
            choose.IsEnabled = false;
            ProfileBox.Items.Add(choose);
            Load();
            ProfileBox.SelectedIndex = ProfileBox.Items.Count - 1;
            newsucc.Interval = TimeSpan.FromSeconds(3);
            newsucc.Tick += newsucc_fnc;
            newsucc.Start();
            NewSucc.Visibility = Visibility.Visible;
    }


        void newsucc_fnc(object sender, EventArgs e) {
            NewSucc.Visibility = Visibility.Hidden;
            newsucc.Tick -= succ_fnc;
        }

    private void SelectProfile(object sender, RoutedEventArgs e) {
            ChooseProfile.IsEnabled = false;
            SliderSpeed.IsEnabled = true;
            SliderSpeedWheel.IsEnabled = true;
            SliderSpeedDbc.IsEnabled = true;
            ComboBoxItem button = (sender as ComboBoxItem);
            LoadProfile(button.Content.ToString());
        }

        private void LoadProfile(string profilename) {
            if (profiles.Count < 1) {
                foreach (Profile profile in profiles) {
                    if (profile.Name == profilename) {
                        SliderSpeed.Value = profile.MouseSensitivity;
                        SliderSpeedWheel.Value = profile.ScrollSensitivity;
                        SliderSpeedDbc.Value = profile.DoubleClickSensitivity;
                        SystemParametersInfo(SPI_SETMOUSESPEED, 0, Convert.ToUInt32(Math.Round(SliderSpeed.Value)), 0);
                        SystemParametersInfo(SPI_SETWHEELSCROLLLINES, Convert.ToUInt32(Math.Round(SliderSpeedWheel.Value)), 0, 0);
                        SystemParametersInfo(SPI_SETDOUBLECLICKTIME, Convert.ToUInt32(Math.Round(SliderSpeedDbc.Value)), 0, 0);
                    }
                }
            } else {
                foreach (Profile profile in WebProfiles) {
                    if (profile.Name == profilename) {
                        SliderSpeed.Value = profile.MouseSensitivity;
                        SliderSpeedWheel.Value = profile.ScrollSensitivity;
                        SliderSpeedDbc.Value = profile.DoubleClickSensitivity;
                        SystemParametersInfo(SPI_SETMOUSESPEED, 0, Convert.ToUInt32(Math.Round(SliderSpeed.Value)), 0);
                        SystemParametersInfo(SPI_SETWHEELSCROLLLINES, Convert.ToUInt32(Math.Round(SliderSpeedWheel.Value)), 0, 0);
                        SystemParametersInfo(SPI_SETDOUBLECLICKTIME, Convert.ToUInt32(Math.Round(SliderSpeedDbc.Value)), 0, 0);
                    }
                }
            }
        }

        private void CreateProfiles() {
            profiles.Add(new Profile("Default", 10, 3, 500));
            profiles.Add(new Profile("Slow", 1, 1, 1));
            profiles.Add(new Profile("Fast", 20, 100, 5000));
            SaveProfiles(profiles);
        }

        private void SaveProfiles(List<Profile> profiles) {
        string jsonToFile = JsonConvert.SerializeObject(profiles, settings);
        File.WriteAllText(path, jsonToFile);
        }

        private void SaveProfile(object sender, RoutedEventArgs e) {
            foreach (Profile profile in profiles) {
                if (profile.Name == ProfileBox.Text ) {
                    profile.ScrollSensitivity = Convert.ToUInt32(Math.Round(SliderSpeed.Value));
                    profile.MouseSensitivity = Convert.ToUInt32(Math.Round(SliderSpeedWheel.Value));
                    profile.DoubleClickSensitivity = Convert.ToUInt32(Math.Round(SliderSpeedDbc.Value));
                }
            }
            succ.Interval = TimeSpan.FromSeconds(3);
            succ.Tick += succ_fnc;
            succ.Start();
            SaveSucc.Visibility = Visibility.Visible;
            SaveProfiles(profiles);
        }

        void succ_fnc(object sender, EventArgs e) {
            SaveSucc.Visibility = Visibility.Hidden;
            succ.Tick -= succ_fnc;
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
            SystemParametersInfo(SPI_SETDOUBLECLICKTIME, svaluedbc, 0, 0);
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
