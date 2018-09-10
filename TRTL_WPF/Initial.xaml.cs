using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Ini.Net;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;

namespace TRTL_WPF
{
    /// <summary>
    /// Interaction logic for Initial.xaml
    /// </summary>
    public partial class Initial
    {
        public IniFile ini = new IniFile("tscripta.ini");
        string filepath = "";
        string remoteurl = "";
        bool remotedaemon = false;
        readonly string root = AppDomain.CurrentDomain.BaseDirectory;
        private void Initialsetup()
        {
            ini.WriteString("Config", "Filepath", "");
            ini.WriteString("Config", "RemoteDaemonaddr", "");
            ini.WriteBoolean("Config", "UseRemote", false);
        }
        private void Setfields()
        {
            filepath = ini.ReadString("Config", "Filepath");
            remoteurl = ini.ReadString("Config", "RemoteDaemonaddr");
            remotedaemon = ini.ReadBoolean("Config", "UseRemote");
            Pathbox.Text = filepath;
            if (filepath != "")
            {
                Default.Visibility = Visibility.Hidden;
                Login.Visibility = Visibility.Visible;
                Transitionframe.Reload();
            }
            remoteurlbox.Text = remoteurl;
            RemoteToggle.IsChecked = remotedaemon ? true : false;
        }
        public Initial()
        {
            if (File.Exists(root + @"\service.log")) File.Delete(root + @"\service.log");
            if (!File.Exists(root + @"\TurtleCoind.exe"))
            {
                MessageBox.Show("Turtlecoind.exe does not exist in the local directory, download it from the Turtlecoin repo");
                this.Close();
            }
            if (!File.Exists(root + @"\turtle-service.exe"))
            {
                MessageBox.Show("turtle-service.exe does not exist in the local directory, download it from the Turtlecoin github repo");
                this.Close();
            }
            if (!File.Exists(root + @"\tscripta.ini")) Initialsetup();
            InitializeComponent();
            Login.Visibility = Visibility.Hidden;
            Setfields();
        }
        private void CreateWallet(object sender, RoutedEventArgs e)
        {
            Walletcreation create = new Walletcreation();
            if ((bool)create.ShowDialog())
            {
                MessageBox.Show(create.Result[0] + create.Result[1]);
            }
        }

        private void Openwallet(object sender, RoutedEventArgs e)
        {
            Default.Visibility = Visibility.Hidden;
            Login.Visibility = Visibility.Visible;
            Transitionframe.Reload();
        }
        private void Backtomain(object sender, RoutedEventArgs e)
        {
            Default.Visibility = Visibility.Visible;
            Login.Visibility = Visibility.Hidden;
            Transitionframe.Reload();
        }
        private void Setdirectory(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog diag = new CommonOpenFileDialog
            {
                EnsureFileExists = true, DefaultDirectory = root
            };
            diag.ShowDialog();
            try { filepath = diag.FileName; }
            catch (InvalidOperationException) { filepath = ""; }
            Pathbox.Text = filepath;
        }

        private void Openwalletfile(object sender, RoutedEventArgs e)
        {
            walletloading.IsActive = true;
            string password = passwordloginbox.Password;
            string path = filepath;
            Thread Thread = new Thread(() =>
            {
                Process Process = new Process();
                Process.StartInfo.FileName = root + @"\turtle-service.exe";
                Process.StartInfo.Arguments = "-w " + path + " -p " + password + " --rpc-legacy-security";
                Process.StartInfo.UseShellExecute = false;
                Process.StartInfo.CreateNoWindow = true;
                if (Process.Start())
                {
                    // Begin redirecting output
                    Thread.Sleep(5000);
                    if (!Process.HasExited) Process.Kill();
                    Thread.Sleep(200);
                    string contents = File.ReadAllText(root + @"\service.log");
                    if (!contents.Contains("Container loaded, view public key"))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => { MessageBox.Show("Wallet or Password is Invalid"); walletloading.IsActive = false; }));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => 
                        {
                            ini.WriteString("Config", "Filepath", filepath);
                            walletloading.IsActive = false;
                            MainWindow Wallet = new MainWindow(path,password, remotedaemon, "http://" + remoteurl);
                            Wallet.Show();
                            this.Close();
                        }));
                    }
                }
            })
            { IsBackground = true };
            Thread.SetApartmentState(ApartmentState.STA);
            Thread.Start();
        }

        private void Updatecheck(object sender, RoutedEventArgs e)
        {
            remotedaemon = (bool)RemoteToggle.IsChecked;
            ini.WriteBoolean("Config", "UseRemote", remotedaemon);
        }


        private void UpdateFielddaemonurl(object sender, MouseEventArgs e)
        {
            remoteurl = remoteurlbox.Text;
            ini.WriteString("Config", "RemoteDaemonaddr", remoteurl);
        }
    }
}