using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace T.Scripta
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : UserControl
    {
        public Overview()
        {
           
            InitializeComponent();
            
        }
        public void UpdateElements()
        {
            try
            {
                WalletHeightind.Text = Turtle.WalletHeight.ToString();
                DaemonHeightind.Text = Turtle.DaemonHeight.ToString();
                NetworkHeightind.Text = Turtle.NetworkHeight.ToString();
                DaemonAddressind.Text = Turtle.DaemonAddress.ToString();
                Peerind.Text = Turtle.PeerCount.ToString();
                Balanceind.Text = Turtle.AvailableBalance.ToString();
                LockedBalanceind.Text = Turtle.LockedAmount.ToString();
            }
            catch { }
        }
        public void UpdateTransactionList(List<TransferData> input)
        {
            Transaction_List.ItemsSource = input;
        }
    }
}
