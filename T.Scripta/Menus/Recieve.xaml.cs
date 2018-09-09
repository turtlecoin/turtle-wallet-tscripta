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
using T.Scripta.Controls;

namespace T.Scripta.Menus
{
    /// <summary>
    /// Interaction logic for Recieve.xaml
    /// </summary>
    public partial class Recieve : UserControl
    {
        public Recieve()
        {
            InitializeComponent();
        }
        public void UpdateElements()
        {
            Address.Text = Turtle.Address;
        }
        private void cpaddr(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Address.Text);
        }

        private void qraddr(object sender, RoutedEventArgs e)
        {
            qrdialog window = new qrdialog(new Uri("https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=" + Address.Text));
            window.ShowDialog();
        }
    }
}
