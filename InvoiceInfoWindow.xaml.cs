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

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for InvoiceInfoWindow.xaml
    /// </summary>
    public partial class InvoiceInfoWindow : Window
    {
        public InvoiceInfoWindow()
        {
            InitializeComponent();
        }

        private void DataGridInvoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
