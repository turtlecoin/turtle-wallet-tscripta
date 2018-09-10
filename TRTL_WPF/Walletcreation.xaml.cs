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
namespace TRTL_WPF
{
    /// <summary>
    /// Interaction logic for Walletcreation.xaml
    /// </summary>
    public partial class Walletcreation
    {
        readonly string root = AppDomain.CurrentDomain.BaseDirectory;
        public Walletcreation()
        {
            InitializeComponent();
        }
        public string[] Result { get; private set; }

        private void Create_wallet(object sender, RoutedEventArgs e)
        {
            string password = Pw2.Password;
            if (Pw1.Password == Pw2.Password)
            {
                if (Pw1.Password.Length <= 5)
                {
                    MessageBox.Show("Password is Too Short! (Must be greater than 5");
                }

                else
                {
                    if (File.Exists(root + @"\" + Filename.Text + ".wallet"))
                    {
                        MessageBox.Show("File name already exists!");
                    }
                    else
                    {
                        // Create wallet process
                        Process Process = new Process();
                        Process.StartInfo.FileName = root + @"\turtle-service.exe";
                        Process.StartInfo.Arguments = "--g -w " + Filename.Text + ".wallet -p " + password; 
                        Process.StartInfo.UseShellExecute = false;
                        Process.StartInfo.RedirectStandardInput = true;
                        Process.StartInfo.RedirectStandardOutput = true;
                        Process.StartInfo.RedirectStandardError = true;
                        Process.StartInfo.CreateNoWindow = true;
                        // Start process
                        if (Process.Start())
                        {
                            Process.WaitForExit();
                            string[] res = { Filename.Text, Pw1.Password };
                            Result = res;
                            this.DialogResult = true;
                            this.Close();
                        }
                        else { MessageBox.Show("Something went wrong"); }
                    }
                
                }
            }
            else { MessageBox.Show("Passwords do not match!"); }
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
    }
}
