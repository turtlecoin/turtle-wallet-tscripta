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
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class Console : UserControl
    {
        public Console()
        {
            InitializeComponent();
        }
        public void Addline(string input)
        {
            Dispatcher.Invoke(() => { Ctrl.AppendText(input + "\n");  });
            
        }
        public void UpdateElements(bool enabled)
        {
            privkbtn.IsEnabled = enabled;
            mnembtn.IsEnabled = enabled;
            Optimizebtn.IsEnabled = enabled;
        }
    }
}
