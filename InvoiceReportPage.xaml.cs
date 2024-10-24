using PartsManager.Model.Entities;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for InvoiceReportPage.xaml
    /// </summary>
    public partial class InvoiceReportPage : UserControl
    {
        public InvoiceReportPage(IEnumerable<InvoicePartInfo> invoiceParts, Invoice invoice, bool isLastPage, string recipient)
        {
            InitializeComponent();
            InvoiceNumberTextBlock.Text = $"Рахунок №{invoice.Id}\nвід {invoice.Date:dd MMMM yyyy}";
            CarTextBlock.Text = $"{invoice.Car.Model.Mark.Name} {invoice.Car.Model.Name}";
            VINCodeTextBlock.Text = invoice.Car.VINCode;
            RecipientTextBlock.Inlines.AddRange(TextBlockParser.ParseInlines(recipient));

            var invoicePartGrid = new InvoicePartGrid(invoiceParts, invoice.SumTotal, isLastPage);
            Grid.SetRow(invoicePartGrid, 4);
            MainGrid.Children.Add(invoicePartGrid);
        }

        public InvoiceReportPage()
        {
            InitializeComponent();
        }
    }

    public static class TextBlockParser
    {
        public static IEnumerable<Inline> ParseInlines(string text)
        {
            var textBlock = (TextBlock)XamlReader.Parse(
                "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">"
                + text
                + "</TextBlock>");

            return textBlock.Inlines.ToList(); // must be enumerated
        }
    }
}
