using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace T.Scripta.Menus
{
    /// <summary>
    /// Interaction logic for Send.xaml
    /// </summary>
    public partial class Send : UserControl
    {
        public double dfee = 0.0, amount = 0.0, total = 0.0;
        public double fee
        {
            get
            {
                if (FeeSelector.SelectedIndex == 0) return 0.1;
                if (FeeSelector.SelectedIndex == 1) return 0.3;
                if (FeeSelector.SelectedIndex == 2) return 0.5;
                if (FeeSelector.SelectedIndex == 3) return 1.0;
                return 0;
            }
        }
        public Send()
        {
            InitializeComponent();
        }
        private void Update()
        {
            try
            {
                amount = (double)Amount.Value;
                dfee = Turtle.RemoteNodeFee;
                total = amount + dfee + fee;
                Amountsendingind.Text = amount.ToString();
                Daemonfeeind.Text = dfee.ToString();
                Transactionfeeind.Text = fee.ToString();
                Transactiontotalind.Text = total.ToString();
            }
            catch { }
            
        }
        private void UpdateTx(object sender, SelectionChangedEventArgs e) { Update(); }

        private void MaxSend(object sender, RoutedEventArgs e)
        {
            Amount.Value = Turtle.AvailableBalance - (Turtle.RemoteNodeFee + fee);
        }

        private void UpdateAmount(object sender, RoutedPropertyChangedEventArgs<double?> e) { Update(); }
        public void UpdateElements()
        {

        }
    }
}
