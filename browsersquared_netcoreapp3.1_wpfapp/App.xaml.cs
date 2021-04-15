// #define DEBUG
#define TRACE

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace browsersquared_netcoreapp3._1_wpfapp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string _passedUri;
     
        // protected override void OnStartup(StartupEventArgs e)
        // {
        //     base.OnStartup(e);
        //     MessageBox.Show($"You have passed:{e.Args.Length} arguments," +
        //                     $" values are {string.Join( ",",e.Args)}");
        //     passedUri = e.Args[0];
        //     
        //     
        // }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
#if DEBUG
            // MessageBox.Show($"You have passed:{e.Args.Length} arguments," +
            //                 $" values are {string.Join( ",",e.Args)}");
#endif
#if TRACE
            Trace.WriteLine($"You have passed:{e.Args.Length} arguments," +
                            $" values are {string.Join( ",",e.Args)}");
#endif
            _passedUri = e.Args.Length > 0 ? e.Args[0] : "";
            MainWindow mainWindow = new MainWindow(_passedUri);
            mainWindow.Show();
        }
    }
    
    public class CustomBrowser
    {
        internal CustomBrowser()
        {
        }

        internal string KeyName { get; set; }

        public string Name { get; set; }

        public string ExecutablePath { get; set; }

        public string IconPath { get; set; }

        public int IconIndex { get; set; }

        public FileVersionInfo Version { get; set; }

        public ReadOnlyDictionary<string, object> FileAssociations { get; set; }

        public ReadOnlyDictionary<string, object> UrlAssociations { get; set; }

        public override string ToString() => this.Name;
    }
}