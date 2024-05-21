using PartsManager.Model.ViewModels;
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

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for InvoicePartGrid.xaml
    /// </summary>
    public partial class InvoicePartGrid : UserControl
    {
        public InvoicePartGrid()
        {
            InitializeComponent();
        }
        public InvoicePartGrid(IEnumerable<InvoicePartInfo> invoiceParts, decimal InvoiceSum, bool IsLastPage)
        {
            InitializeComponent();
            InvoicePartDataGrid.ItemsSource = invoiceParts;
            HeadersDataGrid.ItemsSource = new[]
            {
                new {Index = "№", PartName = "Запчастина", Count = "Кількість", PriceOut = "Ціна", SumOut = "Сума" },
            };
            DataGridInvoiceSum.ItemsSource = new[]
            {
                new { InvoiceSum },
            };
            if(!IsLastPage)
            {
                SumPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
