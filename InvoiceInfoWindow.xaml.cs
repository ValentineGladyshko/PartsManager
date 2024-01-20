using PartsManager.Model.Entities;
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
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using PdfSharp.Xps;
using System.Drawing;
using Spire.Xls;

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for InvoiceInfoWindow.xaml
    /// </summary>
    public partial class InvoiceInfoWindow : Window
    {
        public Invoice LocalInvoice { get; set; }
        public List<Invoice> LocalInvoices { get; set; }
        public InvoiceInfoWindow(Invoice invoice)
        {
            InitializeComponent();
            LocalInvoice = invoice;
            LocalInvoices = new List<Invoice>
            {
                invoice
            };
            DataContext = this;
            var xpsDocument = new XpsDocument("output.xps", FileAccess.Write);
            XpsDocumentWriter xpsdw = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            xpsdw.Write(MainGrid);
            xpsDocument.Close();
            XpsConverter.Convert("output.xps", $"invoice{LocalInvoice.Id}.pdf", 1);
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];
            CellRange range = worksheet.Range[1, 1, 1, 2];
            range.Merge();
            worksheet.Range[1, 1].Value = invoice.Car.FullInfo;
            worksheet.Range[1, 3].Value = invoice.Date.ToString("d");
            worksheet.Range[1, 4].Value = invoice.DeliveryPrice.ToString("C2");           
            worksheet.Range[2, 1].Value = "Запчастина";
            worksheet.Range[2, 2].Value = "К-ть";
            worksheet.Range[2, 3].Value = "Ціна";
            worksheet.Range[2, 4].Value = "Сума";
            
            for (int i = 0; i < LocalInvoice.InvoiceParts.Count; i++)
            {
                worksheet.Range[i + 3, 1].Value = LocalInvoice.InvoiceParts[i].Part.Name;
                worksheet.Range[i + 3, 2].Value = LocalInvoice.InvoiceParts[i].Count.ToString();
                worksheet.Range[i + 3, 3].Value = LocalInvoice.InvoiceParts[i].PriceOut.ToString("C2");
                worksheet.Range[i + 3, 4].Value = LocalInvoice.InvoiceParts[i].SumOut.ToString("C2");
            }
            //worksheet.Range[LocalInvoice.InvoiceParts.Count + 3, 4].Value = invoice.SumTotal.ToString("C2");
            var formula = $"=Sheet1!$D$1+SUM(Sheet1!$D$3:D${LocalInvoice.InvoiceParts.Count + 2})";
            worksheet.Range[LocalInvoice.InvoiceParts.Count + 3, 4].Formula = formula;
            worksheet.SetColumnWidth(1, 25);
            worksheet.SetColumnWidth(2, 10);
            worksheet.SetColumnWidth(3, 15);
            worksheet.SetColumnWidth(4, 15);
            worksheet.SetRowHeight(1, 25);
            worksheet.Range[2, 1, 2, 4].Style.Color = System.Drawing.Color.LightGray;
            worksheet.Range[1, 1, LocalInvoice.InvoiceParts.Count + 3, 4].Borders.Color = System.Drawing.Color.DarkGray;

            workbook.SaveToFile($"invoice{LocalInvoice.Id}.xlsx", ExcelVersion.Version2016);
        }
    }
}
