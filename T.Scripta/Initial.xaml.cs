using System;
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
using System.Threading;
using System.Diagnostics;
using System.IO;
using MahApps.Metro.Controls.Dialogs;

namespace T.Scripta
{
    /// <summary>
    /// Interaction logic for Initial.xaml
    /// </summary>
    public partial class Initial : Page
    {
        private string file = "";
        bool facts = true;
        private Ini.Net.IniFile Ini = new Ini.Net.IniFile("Settings.ini");
        public Initial()
        {
            InitializeComponent();
            if(File.Exists(Ini.ReadString("Wallet", "File")) && System.IO.Path.GetExtension(Ini.ReadString("Wallet","File")) == ".wallet")
            {
                Facts();
                file = Ini.ReadString("Wallet", "File");
                Walletnameind.Text = System.IO.Path.GetFileName(Ini.ReadString("Wallet", "File"));
                Main.Visibility = Visibility.Hidden;
                Wallet.Visibility = Visibility.Visible;
            }
            Daemonaddrbox.Text = Ini.ReadString("Wallet", "DaemonAddr");
            ToggleRemoteDaemon.IsChecked = Ini.ReadBoolean("Wallet", "Remote");
        }

        private void Open_walletbtn(object sender, RoutedEventArgs e)
        {
            var diag = new SelectWallet();
            if ((bool)diag.ShowDialog())
            {
                Facts();
                file = diag.directory;
                Walletnameind.Text = System.IO.Path.GetFileName(diag.directory);
                Main.Visibility = Visibility.Hidden;
                Wallet.Visibility = Visibility.Visible;
                Transition.ReloadTransition();
            }
            
        }
        private void Create_walletbtn(object sender, RoutedEventArgs e)
        {
            var create = new Walletcreation();

            if ((bool)create.ShowDialog())
            {
                (Application.Current.MainWindow as Window).ShowMessageAsync("Wallet Created", "Be sure to backup your keys by loading and exporting the keys!");
            }
        }
        #region Open_Wallet
        private void Returntomainmenubtn(object sender, RoutedEventArgs e)
        {
            
            Main.Visibility = Visibility.Visible;
            Wallet.Visibility = Visibility.Hidden;
            Transition.ReloadTransition();
        }

        private void LoadWallet(object sender, RoutedEventArgs e)
        {
            File.Delete(Turtle.root + @"\service.log");
            Progress.IsActive = true;
            Returntomenubtn.IsEnabled = false;
            Unlockwalletbtn.IsEnabled = false;
            Passwordfield.IsEnabled = false;
            string password = Passwordfield.Password;
            Thread Thread = new Thread(() =>
            {
                Process Process = new Process();
                Process.StartInfo.FileName = Turtle.root + @"\Binaries\turtle-service.exe";
                Process.StartInfo.Arguments = "-w " + file + " -p " + password + " --rpc-legacy-security --server-root "+Turtle.root;
                Process.StartInfo.UseShellExecute = false;
                Process.StartInfo.CreateNoWindow = true;
                if (Process.Start())
                {
                    //7 seconds to wait for wallet to create log that contains success string
                    Thread.Sleep(7000);
                    if (!Process.HasExited) Process.Kill();
                    //.3 seconds for the program to free use of the log for deletion
                    Thread.Sleep(300);
                    string contents = File.ReadAllText(Turtle.root + @"\service.log");
                    if (!contents.Contains("Container loaded, view public key"))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => { MessageBox.Show("Wallet or Password is Invalid"); Progress.IsActive = false; }));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Ini.WriteString("Wallet", "File", file);
                            Main Main = new Main(file, Ini.ReadBoolean("Wallet", "Remote"), password, Ini.ReadString("Wallet", "DaemonAddr"));
                            Progress.IsActive = false;
                            NavigationService nav = NavigationService.GetNavigationService(this);
                            nav.Navigate(Main);
                        }));
                    }
                }
            })
            { IsBackground = true };
            Thread.SetApartmentState(ApartmentState.STA);
            Thread.Start();
           
        }
        #endregion
        private void Facts()
        {
            Task.Run( async() =>
            {
                while (facts)
                    {
                    
                    Dispatcher.Invoke(() => {
                        var rnd = new Random();
                        Fact.Text = (string)Turtle.facts["JSON"]["Facts"][rnd.Next(0, Turtle.facts["JSON"]["Facts"].Count())];
                        FactShower.ReloadTransition();
                    });
                    await Task.Delay(3500);

                }
                MessageBox.Show("Break");
            });
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            Settingsflyout.IsOpen = true;
        }
        private void ApplyandSaveSettings(object sender, RoutedEventArgs e)
        {
            Ini.WriteString("Wallet", "DaemonAddr", Daemonaddrbox.Text);
            Ini.WriteBoolean("Wallet", "Remote", (bool)ToggleRemoteDaemon.IsChecked);
            Settingsflyout.IsOpen = false;
        }
    }
}
