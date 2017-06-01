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



namespace DLLInjector
{
   
        /// <summary>
        /// Lógica de interacción para MainWindow.xaml
        /// </summary>
        /// 

        public partial class MainWindow : Window
    {
        Config config = new Config();
        public MainWindow()
        {
            InitializeComponent();            
        }


        private void DisableControls()
        {
            ProcTextBox.IsEnabled = false;
            DllTextBox.IsEnabled = false;
            CheckConsole.IsEnabled = false;
            CheckProcExit.IsEnabled = false;
            InjectBT.IsEnabled = false;
            BTShadow.Color = (Color)ColorConverter.ConvertFromString("#FF00CC1C"); ;
            CancelBT.Visibility = Visibility.Visible;
        }

        private void CancelInjection()
        {
            ProcTextBox.IsEnabled = true;
            DllTextBox.IsEnabled = true;
            CheckConsole.IsEnabled = true;
            CheckProcExit.IsEnabled = true;
            InjectBT.IsEnabled = true;
            InjectBT.Content = "Inject!";
            BTShadow.Color = (Color)ColorConverter.ConvertFromString("#FF007ACC"); ;
            CancelBT.Visibility = Visibility.Hidden;
        }

        //DragMove Function
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //DragDrop DLL
        private void window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);                
                if (System.IO.Path.GetExtension(files[0]) == ".dll")
                {
                    DllTextBox.Text = System.IO.Path.GetFileName(files[0]);
                    config.DLLPath  =  files[0]; 
                }                               
            }
        }

        //Console Show/Hide
        private void CheckConsole_Click(object sender, RoutedEventArgs e)
        {
            config.Console = CheckConsole.IsChecked.Value;
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
            CheckConsole.IsChecked = config.Console;
            CheckProcExit.IsChecked = config.Close;
        }

        //Inject
        private void InjectBT_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(config.DLLPath))
            {
                config.PrcName = ProcTextBox.Text;
                config.SaveConfig();
                InjectBT.Content = "Waiting for process";
                DisableControls();
                
            }
            else { MessageBox.Show("No se ha encontrado el archivo " + config.DLLPath); }            
        }

        private void CancelBT_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CancelInjection();
        }
    }
}
