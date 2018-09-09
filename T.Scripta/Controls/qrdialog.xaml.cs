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

namespace T.Scripta.Controls
{
    /// <summary>
    /// Interaction logic for qrdialog.xaml
    /// </summary>
    public partial class qrdialog
    {
        public qrdialog(Uri qr)
        {
            InitializeComponent();
            img.Source = new BitmapImage(qr);
        }
    }
}
