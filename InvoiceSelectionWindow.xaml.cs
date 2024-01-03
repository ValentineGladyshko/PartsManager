using PartsManager.Model.Entities;
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
    /// Interaction logic for InvoiceSelectionWindow.xaml
    /// </summary>
    public partial class InvoiceSelectionWindow : Window
    {
        public ICollection<Invoice> LocalInvoices { get; set; }

        public InvoiceSelectionWindow(ICollection<Invoice> invoices)
        {
            InitializeComponent();
            LocalInvoices = invoices;
            DataContext = this;
            SetHandlers();
        }

        public void SetHandlers()
        {
            CreateInvoiceButton.Click += (object sender, RoutedEventArgs e) =>
            {
                InvoiceWindow invoiceWindow = new InvoiceWindow();
                invoiceWindow.Owner = this;

                invoiceWindow.Show();
            };

            InvoiceListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var invoice = InvoiceListBox.SelectedItem as Invoice;
                if (invoice != null)
                {
                    InvoiceWindow invoiceWindow = new InvoiceWindow(invoice);
                    invoiceWindow.Owner = this;

                    invoiceWindow.Show();
                }

                InvoiceListBox.UnselectAll();
            };
        }
    }
}
