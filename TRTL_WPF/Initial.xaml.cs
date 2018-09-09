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

namespace TRTL_WPF
{
    /// <summary>
    /// Interaction logic for Initial.xaml
    /// </summary>
    public partial class Initial
    {
        readonly string root = AppDomain.CurrentDomain.BaseDirectory;

        public Initial()
        {
            if (!File.Exists(root+@"\TurtleCoind.exe"))
            {
                MessageBox.Show("Turtlecoind.exe does not exist in the local directory, download it from the Turtlecoin repo");
                this.Close();
            }
            if (!File.Exists(root + @"\turtle-service.exe"))
            {
                MessageBox.Show("turtle-service.exe does not exist in the local directory, download it from the Turtlecoin github repo");
                this.Close();
            }
            InitializeComponent();
        }
    }
}
