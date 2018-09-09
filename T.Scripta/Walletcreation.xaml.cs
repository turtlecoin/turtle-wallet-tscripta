using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Diagnostics;
using MahApps.Metro.Controls.Dialogs;

namespace T.Scripta
{
    /// <summary>
    /// Interaction logic for Walletcreation.xaml
    /// </summary>
    public partial class Walletcreation
    {
        public bool state = false;
        public bool import = false;
        public Walletcreation()
        {
            InitializeComponent();
            this.Height = 190;
        }
        private void Create_wallet(object sender, RoutedEventArgs e)
        {
            string startupargs = "";
            string password = Pw2.Password;
            if (Pw1.Password == Pw2.Password)
            {
                if (Pw1.Password.Length <= 5)
                {
                    MessageBox.Show("Password is Too Short! (Must be greater than or equal to 5");
                }

                else
                {
                    if (File.Exists(Turtle.root + @"\Wallets\" + Filename.Text + ".wallet"))
                    {
                        this.ShowMessageAsync("Error","File name already exists!");
                    }
                    else
                    {
                        if (!import)
                        {
                            startupargs = @"--g -w Wallets\" + Filename.Text + ".wallet -p " + password + " --server-root " + Turtle.root;
                        }
                        else
                        {
                            if (!state)
                            {
                                if (seed.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count() == 25)
                                {
                                    startupargs = @"--g -w Wallets\" + Filename.Text + ".wallet -p " + password + " --mnemonic-seed \"" + seed.Text.ToLower() + " \"" + " --server-root " + Turtle.root;
                                }
                                else
                                {
                                    this.ShowMessageAsync("Error", "Mnemonic not 25 words long");
                                    return;
                                }
                                
                            }
                            else
                            {
                                if (Spendk.Text.Length == 64 && Viewk.Text.Length == 64)
                                {
                                    startupargs = @"--g -w Wallets\" + Filename.Text + ".wallet -p " + password + " --view-key " + Viewk.Text + " --spend-key " +Spendk.Text + " --server-root " + Turtle.root;
                                }
                                else
                                {
                                    this.ShowMessageAsync("Error", "Invalid Keys");
                                    return;
                                }
                            }
                        }
                        // Create wallet process
                        Process Process = new Process();
                        Process.StartInfo.FileName = Turtle.root + @"\Binaries\turtle-service.exe";
                        Process.StartInfo.Arguments = startupargs;
                        Process.StartInfo.UseShellExecute = false;
                        Process.StartInfo.RedirectStandardInput = true;
                        Process.StartInfo.RedirectStandardOutput = true;
                        Process.StartInfo.RedirectStandardError = true;
                        Process.StartInfo.CreateNoWindow = true;
                        // Start process
                        if (Process.Start())
                        {
                            Process.WaitForExit();
                            if (File.Exists(Turtle.root + @"\Wallets\" + Filename.Text + ".wallet"))
                            {
                                this.DialogResult = true;
                                this.Close();
                            }
                            else
                            {
                                this.ShowMessageAsync("Error", "Failed to created wallet (Invalid Mnemonic/private keys?)");
                            }
                           
                        }
                        else {this.ShowMessageAsync("Error","Something went wrong"); }
                    }

                }
            }
            else {this.ShowMessageAsync("Error","Passwords do not match!"); }
        }

        private void pwintercept(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        

        private void ToggleKey(object sender, RoutedEventArgs e)
        {
            if (!import) { this.Height = 340; import = true; }
            else { this.Height = 190; import = false; }
        }
        private void TabChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Tabb.SelectedIndex == 0)
                {
                    Viewk.Text = "";
                    Spendk.Text = "";
                    state = false;
                }
                else
                {
                    seed.Text = "";
                    state = true;
                }
            }
            catch { }
        }
    }
}
