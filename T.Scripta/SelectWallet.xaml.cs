using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Shapes;

namespace T.Scripta
{
    /// <summary>
    /// Interaction logic for SelectWallet.xaml
    /// </summary>
    public partial class SelectWallet
    {
        public string directory;
        public SelectWallet()
        {
            
            string[] files = System.IO.Directory.GetFiles(Turtle.root + @"\Wallets", "*.wallet");
            files.Reverse();
            
            InitializeComponent();
            if (files.Length == 0) NoWallets.Visibility = Visibility.Visible;
            var Margin = new Thickness(1,1,1,1);
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#CC191919");
            foreach (string file in files)
            {
                var Select = new MahApps.Metro.Controls.Tile();
                Select.Content = System.IO.Path.GetFileName(file);
                Select.Margin = Margin;
                Select.Width = 186;
                Select.Height = 40;
                Select.Background = brush;
                Select.Click += SelectWalletbtn;
                Blah.Children.Add(Select);

            }
        }

        private void SelectWalletbtn(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.Tile sel = (MahApps.Metro.Controls.Tile)sender;
            directory = Turtle.root + @"\Wallets\" +(string)sel.Content;
            this.DialogResult = true;
            this.Close();
        }

        private void Manualsel(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog diag = new CommonOpenFileDialog
            {
                EnsureFileExists = true,
                IsFolderPicker = false,
                
            };
            diag.ShowDialog();
            try
            {
                if (System.IO.Path.GetExtension(diag.FileName) == ".wallet")
                {
                    directory = diag.FileName;
                    this.DialogResult = true;
                    this.Close();
                }
                else { MessageBox.Show("Invalid File Format"); this.DialogResult = false; this.Close(); }
                
            }
            catch (InvalidOperationException) { }
            
        }
    }
}
