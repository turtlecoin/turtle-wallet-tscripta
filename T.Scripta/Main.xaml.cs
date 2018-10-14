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
using T.Scripta.Menus;
using TurtleCoinAPI;

namespace T.Scripta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main
    {
        private Ini.Net.IniFile Ini = new Ini.Net.IniFile("Settings.ini");
        private bool init = false;
        private int SelectedTab = 0;
        // private string name = "";
        public TurtleCoin TRTL;
        public Overview Overview = new Overview();
        public Recieve Receive = new Recieve();
        public Send Send = new Send();
        public Menus.Console Console = new Menus.Console();
        public Main(string file, bool remote, string pw, string addr = "", int port = 11898)
        {
            InitializeComponent();
            Task.Run(() => { Run(file, pw, remote, addr, port); });
            Frame.ReloadTransition();
            Highlight();
            content.Content = Overview;
            Turtle.Save += Shutdown;
            Send.SendBtn.Click += SendTx;
            Walletname.Text = System.IO.Path.GetFileName(file);
            Daemonaddrbox.Text = Ini.ReadString("Wallet", "DaemonAddr");
            ToggleRemoteDaemon.IsChecked = Ini.ReadBoolean("Wallet", "Remote");
        }
        public void Shutdown(object sender, EventArgs e)
        {
            Task.Run(() => { MessageBox.Show("Saving and Cleaning up...", "Closing",MessageBoxButton.OK); });
            TRTL.Exit(true);
        }
        private void OverviewTab(object sender, RoutedEventArgs e) { content.Content = Overview; SelectedTab = 0; Highlight(); }
        private void SendTab(object sender, RoutedEventArgs e) { content.Content = Send; SelectedTab = 1; Highlight(); }
        private void ReceiveTab(object sender, RoutedEventArgs e) { content.Content = Receive; SelectedTab = 2; Highlight(); }
        private void ConsoleTab(object sender, RoutedEventArgs e) { content.Content = Console; SelectedTab = 3; Highlight(); }
        private void SettingsTab(object sender, RoutedEventArgs e) { Settingsflyout.IsOpen = true; }
        static void MainWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }
        private void Highlight()
        {
            switch (SelectedTab)
            {
                case 0:
                    OverviewTabBtn.Opacity = 1;
                    SendTabBtn.Opacity = 0.65;
                    ReceiveTabBtn.Opacity = 0.65;
                    ConsoleTabBtn.Opacity = 0.65;
                    SettingsTabBtn.Opacity = 0.65;

                    break;
                case 1:
                    OverviewTabBtn.Opacity = 0.65;
                    SendTabBtn.Opacity = 1;
                    ReceiveTabBtn.Opacity = 0.65;
                    ConsoleTabBtn.Opacity = 0.65;
                    SettingsTabBtn.Opacity = 0.65;

                    break;
                case 2:
                    OverviewTabBtn.Opacity = 0.65;
                    SendTabBtn.Opacity = 0.65;
                    ReceiveTabBtn.Opacity = 1;
                    ConsoleTabBtn.Opacity = 0.65;
                    SettingsTabBtn.Opacity = 0.65;

                    break;
                case 3:
                    OverviewTabBtn.Opacity = 0.65;
                    SendTabBtn.Opacity = 0.65;
                    ReceiveTabBtn.Opacity = 0.65;
                    ConsoleTabBtn.Opacity = 1;
                    SettingsTabBtn.Opacity = 0.65;

                    break;

            }
        }
        private void Update()
        {
            UpdateElements();
            Task.Run(GetTransactionList);
            Receive.UpdateElements();
            Console.UpdateElements(init);
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
                Overview.Balanceind.Text = Turtle.AvailableBalance.ToString();
                Overview.LockedBalanceind.Text = Turtle.LockedAmount.ToString();
            }
            catch { }
        }
        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                (Application.Current.MainWindow as Window).DragMove();
        }
        #region Wallet
        public void Error(object sender, TurtleCoinErrorEventArgs e)
        {
            Console.Addline("Error: " + e.ErrorCode);
            if (e.ErrorCode == ErrorCode.BAD_CONNECTION)
            {
                Console.Addline("Something went wrong trying to connect to the Daemon, if this persists, try connecting to another daemon or initalize a local daemon");
            }
        }
        public void DaemonLog(object sender, TurtleCoinLogEventArgs e)
        {
            Console.Addline("Daemon: " + e.Message);
        }
        public void WalletLog(object sender, TurtleCoinLogEventArgs e)
        {
            Console.Addline("Wallet: " + e.Message);
        }
        public void OnDaemonConnect(object sender, EventArgs e) { }
        public void OnDaemonDisconnect(object sender, EventArgs e) { TRTL.Exit(true); Console.Addline("FATAL ERROR: Daemon Disconnected!"); }
        public void OnWalletConnect(object sender, EventArgs e) { }
        public void OnWalletDisconnect(object sender, EventArgs e) { TRTL.Exit(true); Console.Addline("FATAL ERROR: Wallet Disconnected!"); }
        public void OnDaemonSynced(object sender, EventArgs e) { }
        public void OnWalletSynced(object sender, EventArgs e) { }
        public async void Run(string path, string pw, bool remote, string remoteurl, int remoteport)
        {

            // Create a new session
            TRTL = new TurtleCoin();
            TRTL.Daemon.RefreshRate = 5000;
            TRTL.Wallet.RefreshRate = 5000;
            // Assign daemon event handlers
            TRTL.Daemon.Log += DaemonLog;
            TRTL.Daemon.Error += Error;
            TRTL.Daemon.OnConnect += OnDaemonConnect;
            TRTL.Daemon.OnSynced += OnDaemonSynced;
            TRTL.Daemon.OnDisconnect += OnDaemonDisconnect;

            // Assign wallet event handlers
            TRTL.Wallet.Log += WalletLog;
            TRTL.Wallet.Error += Error;
            TRTL.Wallet.OnConnect += OnWalletConnect;
            TRTL.Wallet.OnSynced += OnWalletSynced;
            TRTL.Wallet.OnUpdate += Updateui;
            TRTL.Wallet.OnDisconnect += OnWalletDisconnect;
            // Initialize daemon
            if (remote)
            {
                await TRTL.Daemon.InitializeAsync(remoteurl, remoteport);
            }
            else
            {
                await TRTL.Daemon.InitializeAsync(Turtle.root + @"\Binaries\TurtleCoind.exe", 11898);
            }
            // Begin daemon update loop
            await TRTL.Daemon.BeginUpdateAsync();

            // Initialize wallet
            await TRTL.Wallet.InitializeAsync(TRTL.Daemon, Turtle.root + @"\Binaries\turtle-service.exe", path, pw);

            // Begin wallet update loop
            await TRTL.Wallet.BeginUpdateAsync();
             init = true;
    }
        private void Updateui(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => 
            {
                Turtle.Address = TRTL.Wallet.TRTLAddress;
                Turtle.PeerCount = TRTL.Wallet.PeerCount;
                Turtle.WalletHeight = TRTL.Wallet.BlockCount;
                Turtle.DaemonHeight = TRTL.Daemon.Height;
                Turtle.NetworkHeight = TRTL.Daemon.NetworkHeight;
                Turtle.AvailableBalance = TRTL.Wallet.AvailableBalance;
                Turtle.LockedAmount = TRTL.Wallet.LockedAmount;
                Turtle.RemoteNodeAddress = TRTL.Wallet.address;
                Turtle.RemoteNodeFee = TRTL.Wallet.amount;
                Turtle.DaemonAddress = TRTL.Daemon.Address;
                Turtle.Syncronized = TRTL.Wallet.Synced;

                Update();
            }));
        }
        public Task GetTransactionList()
        {
            try
            {
                TRTL.Wallet.SendRequestAsync(RequestMethod.GET_TRANSACTIONS, new JObject { ["firstBlockIndex"] = 1, ["blockCount"] = (int)TRTL.Wallet.BlockCount }, out JObject result);
                if (result.ToString() != Turtle.Cache.ToString())
                {
                    Turtle.Cache = result;
                    List<TransferData> tx = new List<TransferData>();
                    foreach (dynamic a in result["items"])
                    {
                        foreach (dynamic b in a["transactions"])
                        {
                            tx.Add(new TransferData() { Amount = ((double)b["amount"] / 100).ToString() + " TRTL", Date = Turtle.UnixTimeStampToDateTime((double)b["timestamp"]).ToString(), Fee = ((double)b["fee"] / 100).ToString() + " TRTL", TxHash = b["transactionHash"] });
                        }
                    }
                    tx.Reverse();
                    Dispatcher.Invoke(() => { Overview.UpdateTransactionList(tx); });
                }
            }
            catch { Console.Addline("Couldnt retrieve Transaction info!"); }
            return Task.CompletedTask;

        }
        /*public async Task<bool> SendTRTL(string Addr, double Amount, double Fee = .1, string PaymentID = "")
        {
            if (Turtle.Syncronized)
            {
                JObject Params = JObject.Parse(JsonConvert.SerializeObject(new Params() { fee = (int)(Fee * 100), transfers = new List<Transfer> { new Transfer() { address = Addr, amount = (int)(Amount * 100) }, } }));
                if (PaymentID != "") Params["paymentId"] = PaymentID;
                await TRTL.Wallet.SendRequestAsync(RequestMethod.SEND_TRANSACTION, Params, out JObject result);
                if (result["error"] == null)
                {
                    Console.Addline("Sent a transaction");
                    Console.Addline("result:" + result["result"]);
                    return true;
                }

                else
                {
                    Console.Addline("Send Failed: " + result["error"]["message"]);
                    return false;
                }
            }
            else Console.Addline("Cant Send Transaction: Not Synced");
            return false;
        }*/
        private bool SendTRTL()
        {
            if (Turtle.Syncronized)
            {
                JObject Params = JObject.Parse(JsonConvert.SerializeObject(new Params() { fee = (int)(Send.fee * 100), transfers = new List<Transfer> { new Transfer() { address = Send.RecipientAddr.Text, amount = (int)(Send.Amount.Value * 100) }, } }));
                if (Send.PaymentID.Text != "") Params["paymentId"] = Send.PaymentID.Text;
                   TRTL.Wallet.SendRequestAsync(RequestMethod.SEND_TRANSACTION, Params, out JObject result);
                if (result["error"] == null)
                {
                    Console.Addline("Sent a transaction:");
                    Console.Addline("result:" + result["transactionHash"]);
                    return true;
                }

                else
                {
                    Console.Addline("Send Failed: " + result["error"]);
                    return false;
                }
            }
            else Console.Addline("Cant Send Transaction: Not Synced");
            return false;
        }
        public Task SendTransaction()
        {
            string addrsend = Send.RecipientAddr.Text, paymentid = Send.PaymentID.Text;
            double amount = (double)Send.Amount.Value, fee = (double)Send.fee;
            double sum = amount + fee + Turtle.RemoteNodeFee;
            if (addrsend.StartsWith("TRTL") && ((addrsend.Length == 99) || (addrsend.Length == 187)) && addrsend != Turtle.Address)
            {
                if (sum <= Turtle.AvailableBalance)
                {
                    if (amount >= 1 && fee >= 0.1)
                    {
                        if (SendTRTL())
                        {
                            (Application.Current.MainWindow as Window).ShowMessageAsync("Transaction Successful?", "It didnt crash");
                            return Task.CompletedTask;
                        }
                        else (Application.Current.MainWindow as Window).ShowMessageAsync("Transaction Failed?", "Something Went Wrong, Check Console");
                        return Task.CompletedTask;
                    }
                    else (Application.Current.MainWindow as Window).ShowMessageAsync("Transaction Failed", "Amounts must be greater than 1 and Fee must be greater than .1");
                }
                else (Application.Current.MainWindow as Window).ShowMessageAsync("Transaction Failed", "Sum of Amount and Fee is greater than your current balance " + sum.ToString());
            }
            else (Application.Current.MainWindow as Window).ShowMessageAsync("Transaction Failed", "Recipient Address is invalid");
            return Task.CompletedTask;
        }
        private void SendTx(object sender, EventArgs e)
        {
            SendTransaction();
        }

        #endregion

        private void ApplyandSaveSettings(object sender, RoutedEventArgs e)
        {
            Ini.WriteString("Wallet", "DaemonAddr", Daemonaddrbox.Text);
            Ini.WriteBoolean("Wallet", "Remote", (bool)ToggleRemoteDaemon.IsChecked);
            Settingsflyout.IsOpen = false;
        }
    }
    public class Turtle
    {
        public readonly static string root = AppDomain.CurrentDomain.BaseDirectory;
        public static JObject facts, Cache = new JObject();
        public static EventHandler Save,SendTransaction;
        public static string Address { get; set; }
        public static double PeerCount { get; set; }
        public static double WalletHeight { get; set; }
        public static double DaemonHeight { get; set; }
        public static double NetworkHeight { get; set; }
        public static double AvailableBalance { get; set; }
        public static double LockedAmount { get; set; }
        public static string RemoteNodeAddress { get; set; }
        public static double RemoteNodeFee { get; set; }
        public static string DaemonAddress { get; set; }
        public static bool Syncronized { get; set; }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
    public class TransferData
    {
        public string Date { get; set; }
        public string Amount { get; set; }
        public string Fee { get; set; }
        public string TxHash { get; set; }
    }
    public class Transaction
    {
        public List<TransferData> Transfer { get; set; }
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
    }
}
