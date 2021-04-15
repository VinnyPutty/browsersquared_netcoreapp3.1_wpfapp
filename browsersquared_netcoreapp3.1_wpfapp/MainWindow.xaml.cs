// #define DEBUG
#define TRACE
// #define KEEP_AWAKE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using MintPlayer.PlatformBrowser;



namespace browsersquared_netcoreapp3._1_wpfapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Browser> _browsers;
        private Browser _defaultBrowser;
        private Dictionary<string, string> _browserPaths;
        private Dictionary<string, List<string>> _browserNames;
        private Dictionary<Key, Action<object, RoutedEventArgs>> _keyMap;

        // ReSharper disable once MemberCanBePrivate.Global
        //  cannot be private because of Binding
        public string PassedUri { get; set; }

        public MainWindow()
        {
            Setup("");
        }
        
        public MainWindow(string passedUri="")
        {
            Setup(passedUri);
        }

        private void Setup(string passedUri = "")
        {
#if TRACE
            Trace.WriteLine("Now let us begin!");
#endif
            Build_Browser_Name_Dict();
            Build_KeyMap_Dict();
            _defaultBrowser = PlatformBrowser.GetDefaultBrowser();
            _browsers = new List<Browser>();
            _browserPaths = new Dictionary<string, string>();
            foreach (var installedBrowser in PlatformBrowser.GetInstalledBrowsers())
            {
                this._browsers.Add(installedBrowser);
                // ReSharper disable once PossibleNullReferenceException
                foreach (var (browserName, altNames) in _browserNames.Where(browserName => _browserNames
                    .GetValueOrDefault(browserName.Key)
                    .Contains(installedBrowser.Name)))
                {
                    if (!_browserPaths.ContainsKey(browserName)) _browserPaths.Add(browserName, installedBrowser.ExecutablePath);
                    break;
                }
#if TRACE
                Trace.WriteLine($"Browser: {installedBrowser.Name}");
                Trace.WriteLine($"Executable: {installedBrowser.ExecutablePath}");
                Trace.WriteLine($"Icon path: {installedBrowser.IconPath}");
                Trace.WriteLine($"Icon index: {installedBrowser.IconIndex}");
                Trace.WriteLine("");
#endif
            }
            // for browser in PlatformBrowser.GetInstalledBrowsers();

            Brave_Detector();

#if TRACE
            // Trace.WriteLine(_browserPaths);

            foreach (var (name, names) in _browserNames)
            {
                Trace.WriteLine($"{name}: {string.Join(", ", names)}");
            }
            Trace.WriteLine("");
            
            foreach (var (name, path) in _browserPaths)
            {
                Trace.WriteLine($"{name}: {path}");
            }
            Trace.WriteLine("");
#endif

            DataContext = this;
            PassedUri = passedUri != "" ? passedUri : "(URL not received)";

            InitializeComponent();
        }

        private void Build_Browser_Name_Dict()
        {
            _browserNames = new Dictionary<string, List<string>>
            {
                {"Chrome", new List<string>(new[] {"Google Chrome", "Chrome"})},
                {"Edge", new List<string>(new[] {"Microsoft Edge", "Edge"})},
                {"Brave", new List<string>(new[] {"Brave Browser", "Brave"})},
                {"Firefox", new List<string>(new[] {"Mozilla Firefox", "Firefox"})},
                {"Vivaldi", new List<string>(new[] {"Vivaldi"})},
                {"Opera", new List<string>(new[] {"Opera Stable", "Opera"})}
            };
        }

        private void Build_KeyMap_Dict()
        {
            _keyMap = new Dictionary<Key, Action<object, RoutedEventArgs>>
            {
                {Key.Escape, Cancel_OnClick},
                
                {Key.C, Chrome_OnClick},
                {Key.E, Edge_OnClick},
                {Key.B, Brave_OnClick},
                {Key.F, Firefox_OnClick},
                {Key.V, Vivaldi_OnClick},
                {Key.O, Opera_OnClick},
            };
        }

        #region Custom Browser Detectors

        private bool Brave_Detector()
        {
            const string defaultBravePath = @"C:\Users\v\AppData\Local\BraveSoftware\Brave-Browser\Application\brave.exe";
            if (_browserPaths.ContainsKey("Brave")) return true;
            try
            {
                if (!File.Exists(defaultBravePath)) throw new FileNotFoundException();
                CustomBrowser braveBrowser = new CustomBrowser
                {
                    KeyName = "Brave",
                    Name = "Brave",
                    ExecutablePath = defaultBravePath,
                    Version = FileVersionInfo.GetVersionInfo(defaultBravePath),
                    // IconPath = flag ? strArray[0] : str,
                    // IconIndex = !flag ? 0 : (strArray.Length > 1 ? Convert.ToInt32(strArray[1]) : 0),
                    // FileAssociations = readOnlyDictionary1,
                    // UrlAssociations = readOnlyDictionary2
                };
                // _browsers.Add((Browser) braveBrowser);
                _browserPaths.Add("Brave", defaultBravePath);
            }
            catch (FileNotFoundException e)
            {
#if TRACE
                Trace.WriteLine(e);
#endif
            }

            return false;
        }
        
        #endregion

        private bool Validate_Uri(string uri)
        {
            // TODO: implement uri validation
            return true;
        }
        
        private string Validate_Uri(string passedUri, string defaultUri)
        {
            // TODO: Implement default uri selection if passedUri is not valid
            return passedUri != "(URL not received)" ? passedUri : defaultUri;
        }

        private string Validate_Uri(string[] uris)
        {
            // TODO: implement select first valid uri
            return "";
        }
        
        private bool Open_Uri_Specific_Browser(string browser, string uri)
        {
            var browserPath = _browserPaths.GetValueOrDefault(browser, null);
            if (browserPath == null) return false;
            uri = Validate_Uri(PassedUri, uri);
            Process.Start(browserPath, uri);
            Trace.WriteLine($"Opening '{uri}' in {browser}.");
            
#if !KEEP_AWAKE
            Close();
#endif
            
            return true;
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Chrome_OnClick(object sender, RoutedEventArgs e)
        {
            // var chromePath = _browserPaths.GetValueOrDefault("Chrome", null);
            // if (chromePath == null) return;
            // //shellExecute(chromePath + "https://chrome.google.com");
            // Process.Start(chromePath, "https://chrome.google.com");
            // Trace.WriteLine($"Opening 'https://chrome.google.com' in Chrome...");

            Open_Uri_Specific_Browser("Chrome", "https://chrome.google.com");
        }

        private void Edge_OnClick(object sender, RoutedEventArgs e)
        {
            Open_Uri_Specific_Browser("Edge", "https://www.microsoft.com/en-us/edge");
        }

        private void Brave_OnClick(object sender, RoutedEventArgs e)
        {
            Open_Uri_Specific_Browser("Brave", "https://brave.com/");
        }
        
        private void Firefox_OnClick(object sender, RoutedEventArgs e)
        {
            Open_Uri_Specific_Browser("Firefox", "https://www.mozilla.org/en-US/firefox/new/");
        }
        
        private void Vivaldi_OnClick(object sender, RoutedEventArgs e)
        {
            Open_Uri_Specific_Browser("Vivaldi", "https://vivaldi.com/");
        }
        
        private void Opera_OnClick(object sender, RoutedEventArgs e)
        {
            Open_Uri_Specific_Browser("Opera", "https://www.opera.com/");
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            _keyMap.GetValueOrDefault(e.Key, (o, args) => { })?.Invoke(sender, e);
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
    }
}