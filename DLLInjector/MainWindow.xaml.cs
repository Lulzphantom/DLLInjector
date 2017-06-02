using System;
using System.IO;
using System.Collections.Generic;
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
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;
using EasyHook;
using System.Runtime.Remoting;
using System.Diagnostics;
using System.Windows.Threading;

namespace DLLInjector
{

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 
    public class HackInterface : MarshalByRefObject
    {
        DateTime localDate = DateTime.Now;
        public void IsInstalled(Int32 InClientPID)
        {
            return;
        }

        public void WriteConsole(String Write)
        {
            Console.WriteLine(localDate.ToShortTimeString() + " - " + Write);
        }

        public void ErrorHandler(Exception err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(localDate.ToShortTimeString() + " - Error: {0}", err.ToString());
            Console.ResetColor();
        }

    }
    public partial class MainWindow : Window
    {
        private static string ChannelName = null;

        Config config = new Config();
        DateTime localDate = DateTime.Now;
        DispatcherTimer InjectWait = new DispatcherTimer();
        DispatcherTimer CloseWait = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void CancelBT_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CancelInjection();
        }

        //OpenFileDialog DLL
        private void SearchDll_Click(object sender, RoutedEventArgs e)
        {
            var FileDialog = new OpenFileDialog();
            FileDialog.Multiselect = false;
            FileDialog.Filter = "DLL files (*.dll)|*.dll";
            if (FileDialog.ShowDialog() == true)
            {
                config.DLLPath = FileDialog.FileName;
                DllTextBox.Text = System.IO.Path.GetFileName(FileDialog.FileName);
                DllTextBox.ToolTip = config.DLLPath;
            }
        }

        private void DisableControls()
        {
            ProcTextBox.IsEnabled = false;            
            CheckConsole.IsEnabled = false;
            CheckProcExit.IsEnabled = false;
            SearchDll.IsEnabled = false;
            InjectBT.IsEnabled = false;
            BTShadow.Color = (Color)ColorConverter.ConvertFromString("#FF00CC1C"); 
            CancelBT.Visibility = Visibility.Visible;
        }

        private void CancelInjection()
        {
            ProcTextBox.IsEnabled = true;            
            CheckConsole.IsEnabled = true;
            SearchDll.IsEnabled = true;
            CheckProcExit.IsEnabled = true;
            InjectBT.IsEnabled = true;
            InjectBT.Content = "Inject!";
            BTShadow.Color = (Color)ColorConverter.ConvertFromString("#FF007ACC"); ;
            CancelBT.Visibility = Visibility.Hidden;
            InjectWait.Stop();
            Console.WriteLine(localDate.ToShortTimeString() + " - Injection canceled");
        }

        //DragMove Function
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //DragDrop DLL
        private void window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && SearchDll.IsEnabled == true)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);                
                if (System.IO.Path.GetExtension(files[0]) == ".dll")
                {
                    DllTextBox.Text = System.IO.Path.GetFileName(files[0]);
                    config.DLLPath  =  files[0];
                    DllTextBox.ToolTip = config.DLLPath;
                }                               
            }
        }

        //Console Show/Hide
        private void CheckConsole_Click(object sender, RoutedEventArgs e)
        {
            config.console = CheckConsole.IsChecked.Value;
            if (CheckConsole.IsChecked == true)
            {
                ConsoleManager.Show();                
            }
            else
            {
                ConsoleManager.Hide();
                
            }
        }
        //CheckBox Proc closed
        private void CheckProcExit_Click(object sender, RoutedEventArgs e)
        {
            config.Close = CheckProcExit.IsChecked.Value;
        }

        //Enable InjectBT
        private void ProcTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ProcTextBox.Text.Length> 3 && DllTextBox.Text.Length > 3)
            {
                InjectBT.IsEnabled = true;
                BTShadow.Opacity = 1;
            }
            else
            {
                InjectBT.IsEnabled = false;
                BTShadow.Opacity = 0;
            }
        }
        private void DllTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DllTextBox.ToolTip = config.DLLPath;
            if (ProcTextBox.Text.Length > 3 && DllTextBox.Text.Length > 3)
            {
                InjectBT.IsEnabled = true;
                BTShadow.Opacity = 1;
            }
            else
            {
                InjectBT.IsEnabled = false;
                BTShadow.Opacity = 0;
            }
        }

        //Load config
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            config.CheckConfig();
            ProcTextBox.Text = config.PrcName;
            DllTextBox.Text = System.IO.Path.GetFileName(config.DLLPath);
            CheckConsole.IsChecked = config.console;            
            if (config.console == true)
            {
                ConsoleManager.Show();
            }
            CheckProcExit.IsChecked = config.Close;
            InjectWait.Tick += InjectMotherFucker;
            InjectWait.Interval = new TimeSpan(0,0,0,2);
            DllTextBox.ToolTip = config.DLLPath;            
            CloseWait.Tick += WaitToClose;
            InjectWait.Interval = new TimeSpan(0, 0, 0, 2);
        }

        //InjectBT
        private void InjectBT_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(config.DLLPath))
            {
                config.PrcName = ProcTextBox.Text;
                config.SaveConfig();
                InjectBT.Content = "Waiting for process";
                DisableControls();
                Console.WriteLine(localDate.ToShortTimeString() + " - Waiting for process " + config.PrcName);                
                InjectWait.Start();
            }
            else { MessageBox.Show("File not found: " + config.DLLPath);
                Console.WriteLine(localDate.ToShortTimeString() + " - File not found: " + config.DLLPath);
            }
        }

        //InjectProcess 
        private void InjectMotherFucker(object sender, EventArgs e)
        {
            try
            {                
                int pid = -1;
                Process[] procs = Process.GetProcessesByName(config.PrcName);
                if (procs.Length <= 0)
                {
                    Console.WriteLine(localDate.ToShortTimeString() + " - Proccess doesn't exists");                    
                    return;
                }
                pid = procs[0].Id;
                RemoteHooking.IpcCreateServer<HackInterface>(ref ChannelName, WellKnownObjectMode.Singleton);
                RemoteHooking.Inject(pid, InjectionOptions.DoNotRequireStrongName, config.DLLPath, config.DLLPath, new Object[] { ChannelName });
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(localDate.ToShortTimeString() + " - DLL Injected!");
                Console.ResetColor();
                InjectBT.Content = "Dll successfully injected.";
                BTShadow.Color = (Color)ColorConverter.ConvertFromString("#FFCC0000");
                if (config.Close == true) { CloseWait.Start(); }
                InjectWait.Stop();
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(localDate.ToShortTimeString() + " - There was an error while connecting to target: \r\n {0}", err.ToString());
                CancelInjection();
                Console.ResetColor();
            }
        }

        //Wait to process close
        private void WaitToClose(object sender, EventArgs e)
        {
            Process[] procs = Process.GetProcessesByName(config.PrcName);
            if (procs.Length == 0)
            {
                this.Close();
            }
        }

        //ExitBT
        private void CloseLb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void CloseLb_MouseEnter(object sender, MouseEventArgs e)
        {
            ExitShadow.Opacity = 1;
        }
        private void CloseLb_MouseLeave(object sender, MouseEventArgs e)
        {
            ExitShadow.Opacity = 0;
        }
    }
}
