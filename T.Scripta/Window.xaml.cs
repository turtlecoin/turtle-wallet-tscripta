using System.Windows;
using System.Windows.Controls;

namespace T.Scripta
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window
    {
        public Window()
        {
            System.Diagnostics.Process[] pname = System.Diagnostics.Process.GetProcessesByName("turtle-service");
            if (pname.Length != 0) {
                MessageBox.Show("turtle-service is already running! close it by task manager or the program it is using", "Already Running Process"); this.Close();
            }
            if (!System.IO.File.Exists(Turtle.root + @"\Settings.ini")) initial();
            System.IO.Directory.CreateDirectory(Turtle.root + @"\Wallets");
            System.IO.Directory.CreateDirectory(Turtle.root + @"\Binaries");
            if (!System.IO.File.Exists(Turtle.root + @"\Binaries\TurtleCoind.exe")) {
                MessageBox.Show("Turtlecoind not found! download the latest binaries and place it in the Binaries Folder.", "Missing Binaries");
                System.Diagnostics.Process.Start("https://github.com/turtlecoin/turtlecoin/releases");
                this.Close();
            }
            if (!System.IO.File.Exists(Turtle.root + @"\Binaries\turtle-service.exe"))
            {
                MessageBox.Show("turtle-service not found! download the latest binaries and place it in the Binaries Folder.", "Missing Binaries");
                System.Diagnostics.Process.Start("https://github.com/turtlecoin/turtlecoin/releases");
                this.Close();
            }
            InitializeComponent();
            using (var net = new System.Net.WebClient())
            {
                Turtle.facts = Newtonsoft.Json.Linq.JObject.Parse(net.DownloadString("https://raw.githubusercontent.com/turtlecoin/turtle-facts-json/master/facts.json"));
            }
            Frame.Content = new Initial();
        }
        private void Save(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Turtle.Save?.Invoke(null, null);
        }
        private void initial()
        {
            var create = new Ini.Net.IniFile("Settings.ini");
            create.WriteString("Wallet", "File", "");
            create.WriteString("Wallet", "DaemonAddr", "");
            create.WriteBoolean("Wallet", "Remote", false);
        }
    }
}
