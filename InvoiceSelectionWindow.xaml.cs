using PartsManager.Model.Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PartsManager
{
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
            CreateInvoiceButton.Click += delegate
            {
                var invoiceWindow = new InvoiceWindow(false)
                {
                    Owner = this
                };

                invoiceWindow.Show();
            };

            DataGridInvoices.SelectionChanged += delegate
            {
                if (DataGridInvoices.SelectedItem is Invoice invoice)
                {
                    var invoiceWindow = new InvoiceWindow(invoice)
                    {
                        Owner = this
                    };

                    invoiceWindow.Show();
                }

                DataGridInvoices.UnselectAll();
            };
        }
    }
}
