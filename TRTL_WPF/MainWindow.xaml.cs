using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using TurtleCoinAPI;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MahApps.Metro.Controls.Dialogs;
using System.Net;
using System.Text.RegularExpressions;

namespace TRTL_WPF
{
    public partial class MainWindow
    {
        #region Walletstuff
        readonly string root = AppDomain.CurrentDomain.BaseDirectory;
        public TurtleCoin TRTLSess;
        public void Appendtxtasync( string input)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { Console.AppendText("\n" +input);  Console.ScrollToEnd(); }));
            }
        public void DaemonLog(object sender, TurtleCoinLogEventArgs e)
        {

            Appendtxtasync("Daemon: "+ e.Message);
        }
        public void WalletLog(object sender, TurtleCoinLogEventArgs e)
        {
            Appendtxtasync("Wallet: "+ e.Message);
        }
        // Print errors to console
        public void Error(object sender, TurtleCoinErrorEventArgs e)
        {
            Appendtxtasync("Error: " + e.ErrorCode);
        }
        // Daemon update event
        public void OnDaemonUpdate(object sender, EventArgs e)
        {
            // Daemon must read as synced to send requests
            // if (!(sender as Daemon).Synced)
            //   Console.WriteLine("Daemon:\tSyncing - {0} / {1}", (sender as Daemon).Height, (sender as Daemon).NetworkHeight);
        }
        // Wallet update event
        public void OnWalletUpdate(object sender, EventArgs e)
        {
            if (!(sender as Wallet).Synced)
                Appendtxtasync("Wallet: " + (sender as Wallet).BlockCount + "/" + (sender as Wallet).KnownBlockCount);
            else
                Appendtxtasync("Available Balance: " + (sender as Wallet).AvailableBalance + ", Locked Amount:" + (sender as Wallet).LockedAmount);
        }
        public void OnDaemonConnect(object sender, EventArgs e) { }
        public void OnDaemonDisconnect(object sender, EventArgs e) { TRTLSess.Exit(true); Appendtxtasync("FATAL ERROR: Daemon Disconnected!"); }
        public void OnWalletConnect(object sender, EventArgs e) { }
        public void OnWalletDisconnect(object sender, EventArgs e) { }
        public void OnDaemonSynced(object sender, EventArgs e) { }
        public void OnWalletSynced(object sender, EventArgs e) { }
        public async void Run()
        {
            // Create a new session
            TRTLSess = new TurtleCoin();
            TRTLSess.Daemon.RefreshRate = 5000;
            TRTLSess.Wallet.RefreshRate = 5000;
            // Assign daemon event handlers
            TRTLSess.Daemon.Log += DaemonLog;
            TRTLSess.Daemon.Error += Error;
            TRTLSess.Daemon.OnConnect += OnDaemonConnect;
            TRTLSess.Daemon.OnSynced += OnDaemonSynced;
            TRTLSess.Daemon.OnUpdate += Updateui;
            TRTLSess.Daemon.OnDisconnect += OnDaemonDisconnect;

            // Assign wallet event handlers
            TRTLSess.Wallet.Log += WalletLog;
            TRTLSess.Wallet.Error += Error;
            TRTLSess.Wallet.OnConnect += OnWalletConnect;
            TRTLSess.Wallet.OnSynced += OnWalletSynced;
            TRTLSess.Wallet.OnUpdate += OnWalletUpdate;
            TRTLSess.Wallet.OnDisconnect += OnWalletDisconnect;

            // Initialize daemon
            //await TRTLSess.Daemon.InitializeAsync("http://localhost", 11898);
            await TRTLSess.Daemon.InitializeAsync("http://public.turtlenode.io", 11898);
            //await TRTLSess.Daemon.InitializeAsync(root + @"\TurtleCoind.exe", 11898);
            // Begin daemon update loop
            await TRTLSess.Daemon.BeginUpdateAsync();

            // Initialize wallet, creating container if it doesn't exist
            if (!File.Exists(root + @"\abc.wallet")) await TRTLSess.Wallet.CreateOrInitializeAsync(TRTLSess.Daemon, root + @"\turtle-service.exe", root + @"\abc.wallet", "abc");
            else await TRTLSess.Wallet.InitializeAsync(TRTLSess.Daemon, root + @"\turtle-service.exe", root + @"\abc.wallet", "abc");

            // Begin wallet update loop
            await TRTLSess.Wallet.BeginUpdateAsync();
            initialized = true;
            //Thread.Sleep(1500);
            //geataddress();
        }
        public bool SendTRTL(string Addr, double Amount, double Fee = .01, int Mixin = 7, string PaymentID = "")
        {
            if (TRTLSess.Wallet.Synced)
            {
                JObject Params = JObject.Parse(JsonConvert.SerializeObject(new Params() { anonymity = Mixin, fee = (int)(Fee * 100), transfers = new List<Transfer> { new Transfer() { address = Addr, amount = (int)(Amount * 100) }, } }));
                if (PaymentID != "") Params["paymentId"] = PaymentID;
                TRTLSess.Wallet.SendRequestAsync(RequestMethod.SEND_TRANSACTION, Params, out JObject result);
                if (result["error"] == null)
                {
                    Appendtxtasync("Sent a transaction");
                    Appendtxtasync("result:" + result["result"]);
                    return true;
                }

                else
                {
                    Appendtxtasync("Send Failed: " + result["error"]["message"]);
                    return false;
                }
            }
            else Appendtxtasync("Cant Send Transaction: Not Synced");
            return false;
        }
        #endregion
        #region gui
        public bool initialized = false;
        public int Current_Page = -1;
        public void Changepage(int page)
        {
            if (Current_Page != page)
            {
                if (page == 0)
                {
                    Wallet_Page.Visibility = Visibility.Visible;
                    Send_Page.Visibility = Visibility.Hidden;
                    Recieve_Page.Visibility = Visibility.Hidden;
                    Current_Page = 0;
                }
                if (page == 1)
                {
                    Wallet_Page.Visibility = Visibility.Hidden;
                    Send_Page.Visibility = Visibility.Visible;
                    Recieve_Page.Visibility = Visibility.Hidden;
                    Current_Page = 1;
                }
                if (page == 2)
                {
                    Wallet_Page.Visibility = Visibility.Hidden;
                    Send_Page.Visibility = Visibility.Hidden;
                    Recieve_Page.Visibility = Visibility.Visible;
                    Current_Page = 2;
                }
                Frame.Reload();
            }
        }
        private void Updateui(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                Addressbox.Text = TRTLSess.Wallet.TRTLaddr;
                Balind.Text = TRTLSess.Wallet.AvailableBalance.ToString();
                lockedind.Text = TRTLSess.Wallet.LockedAmount.ToString();
                Syncind.Text = TRTLSess.Daemon.NetworkHeight - 100 < TRTLSess.Daemon.Height ? "True" : "False";
                peerind.Text = TRTLSess.Daemon.WhitePeerlistSize.ToString();
                networkheightind.Text = TRTLSess.Daemon.NetworkHeight.ToString();
                daemonheightind.Text = TRTLSess.Daemon.Height.ToString();
                WalletHeight.Text = TRTLSess.Wallet.BlockCount.ToString();
                connectedtodaemon.Text = TRTLSess.Daemon.Connected ? "Connected" : "Disconnected";
            }));
        }
        public MainWindow()
        {
            InitializeComponent();
            Changepage(0);
            Thread Thread = new Thread(() =>
            {
                Run();
            })
            { IsBackground = true };
            Thread.SetApartmentState(ApartmentState.STA);
            Thread.Start();
        }
        #endregion
        #region Events
        private void Open_Settings(object sender, RoutedEventArgs e)
        {
           var blah = this.ShowMessageAsync("Test","balgdflgdrlg");
            //Settings window = new Settings();
            //window.ShowDialog();

        }
        private void Menuitemclick(object sender, RoutedEventArgs e)
        {
            TreeViewItem obj = (TreeViewItem)sender;
            string a = (string)obj.ToolTip;
            MessageBox.Show(a);
        }
        private void Walletbtn_Click(object sender, RoutedEventArgs e)
        {
            if (initialized) Changepage(0);
        }
        private void Sendbtn_Click(object sender, RoutedEventArgs e)
        {
            if (initialized) Changepage(1);
        }

        private void Recievebtn_Click(object sender, RoutedEventArgs e)
        {
            if (initialized) Changepage(2);
        }

        private void Save(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TRTLSess.Exit(true);
        }

        #endregion
        private void cpaddr(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Addressbox.Text);
        }

        private void qraddr(object sender, RoutedEventArgs e)
        {
            qrdialog window = new qrdialog(new Uri("https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=" + Addressbox.Text));
            window.ShowDialog();
        }

        private void SendTX(object sender, RoutedEventArgs e)
        {
            string addrsend = recipientaddrsendfield.Text, paymentid = paymentidsendfield.Text;
            double amount = (double)Amountsendfield.Value, fee = (double)Feesendfield.Value;
            double sum = amount + fee;
             if ( addrsend.StartsWith("TRTL") && ((addrsend.Length == 99) || (addrsend.Length == 187))  && addrsend != TRTLSess.Wallet.TRTLaddr)
             {
                 if (sum <= TRTLSess.Wallet.AvailableBalance)
                 {
                     if (amount >= 1 && fee >= 0.1)
                     {
                         if(SendTRTL(addrsend, amount, fee,7,paymentid))
                        {
                            this.ShowMessageAsync("Transaction Successful?", "It didnt crash");
                        }
                         else this.ShowMessageAsync("Transaction Failed?", "Something Went Wrong, Check Console");
                    }
                     else this.ShowMessageAsync("Transaction Failed", "Amounts must be greater than 1 and Fee must be greater than .1");
                 }
                 else this.ShowMessageAsync("Transaction Failed", "Sum of Amount and Fee is greater than your current balance");
             }
             else this.ShowMessageAsync("Transaction Failed", "Recipient Address is invalid");
        }

        private void updatetx(object sender, MouseEventArgs e)
        {
            amounttx.Text = Amountsendfield.Value.ToString();
            dfeetx.Text = TRTLSess.Daemon.remotenodefee.ToString();
            dfeetxvis.Visibility = TRTLSess.Daemon.remotenodefee == 0 ? Visibility.Collapsed : Visibility.Visible;
            Feetx.Text = Feesendfield.Value.ToString();
            sumoftx.Text = (Amountsendfield.Value + TRTLSess.Daemon.remotenodefee + Feesendfield.Value).ToString();
        }
    }
    public class Transfer
    {
        public string address { get; set; }
        public int amount { get; set; }
    }

    public class Params
    {
        public List<Transfer> transfers { get; set; }
        public int fee { get; set; }
        public int anonymity { get; set; }
    }
}
/* Code that probably will be in future things 
 <TreeView Name="txlist" Height="233">
                            <TreeViewItem Header="Direction | Amount | Confirmations | Address" Background="#FF3A3A3A" TextOptions.TextHintingMode="Animated"/>
                        </TreeView>

    private void AppendTx(bool isrecieve, string addr, double amount, string txid)
        {
            string a1 = isrecieve ? "Incoming" : "Sent";
            TreeViewItem item = new TreeViewItem { Header = (a1 + " | " + amount + " TRTL | " + addr) };
            item.MouseDoubleClick += Menuitemclick;
            item.ToolTip = "Hash: " + txid;
            txlist.Items.Add(item);
        }
 */
