using PartsManager.Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Xps.Packaging;
using PdfSharp.Xps;
using Spire.Xls;
using System.Reflection;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using PartsManager.Model.ViewModels;
using System.Windows.Markup;
using System.Windows.Input;
using System.IO.Packaging;

namespace PartsManager
{
    public partial class MyInvoiceInfoWindow : Window
    {
        public MyInvoiceInfoWindow(Invoice invoice)
        {
            int index = 1;
            var invoiceParts = (from invoicePart in invoice.InvoiceParts.ToList()
                         select new InvoicePartInfo
                         {
                             Index = index++.ToString(),
                             PartName = invoicePart.Part.Name,
                             Count = invoicePart.Count.ToString(),
                             PriceOut = invoicePart.PriceOutWithDelivery.ToString("C2"),
                             SumOut = (invoicePart.PriceOut * invoicePart.Count).ToString("C2"),
                         }).ToList();
            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = new Size(794, 1123);
            var mainPage = new FixedPage
            {
                Height = document.DocumentPaginator.PageSize.Height,
                Width = document.DocumentPaginator.PageSize.Width
            };
            var reportPage = new InvoiceReportPage();
            const int RowsPerFirstPage = 30;
            const int RowsPerPage = 50;
            if (invoiceParts.Count() > RowsPerFirstPage)
            {
                var firstPartSegment = invoiceParts.GetRange(0, RowsPerFirstPage);
                reportPage = new InvoiceReportPage(firstPartSegment, invoice, false);

                mainPage.Children.Add(reportPage);
                PageContent pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(mainPage);
                document.Pages.Add(pageContent);

                var invoicePartsSegments = new List<List<InvoicePartInfo>>();
                for (int i = RowsPerFirstPage; i < invoiceParts.Count; i += RowsPerPage)
                {
                    invoicePartsSegments.Add(invoiceParts.GetRange(i, Math.Min(RowsPerPage, invoiceParts.Count - i)));
                }

                var partGrids = invoicePartsSegments.GetRange(0, invoicePartsSegments.Count - 1).ConvertAll(item => new InvoicePartGrid(item, invoice.SumTotal2, false));
                partGrids.Add(new InvoicePartGrid(invoicePartsSegments.Last(), invoice.SumTotal2, true));

                foreach (var partGrid in partGrids)
                {
                    var gridPage = new FixedPage
                    {
                        Height = document.DocumentPaginator.PageSize.Height,
                        Width = document.DocumentPaginator.PageSize.Width
                    };
                    gridPage.Children.Add(partGrid);
                    PageContent gridPageContent = new PageContent();
                    ((IAddChild)gridPageContent).AddChild(gridPage);
                    document.Pages.Add(gridPageContent);
                }
            }
            else
            {
                reportPage = new InvoiceReportPage(invoiceParts, invoice, true);
                mainPage.Children.Add(reportPage);
                PageContent pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(mainPage);
                document.Pages.Add(pageContent);
            }

            InitializeComponent();


            var directory = AppDomain.CurrentDomain.BaseDirectory + "reports";
            Directory.CreateDirectory(directory);
            var xpsDocument = new XpsDocument("output.xps", FileAccess.Write);
            var xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            xpsDocumentWriter.Write(document);
            xpsDocument.Close();
            DocumentInvoice.Document = document;
            XpsConverter.Convert("output.xps", $"reports/invoice{invoice.Id}.pdf", 1);
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets[0];
            var range = worksheet.Range[1, 1, 1, 2];
            range.Merge();
            worksheet.Range[1, 1].Value = invoice.Car.FullInfo;
            worksheet.Range[1, 3].Value = invoice.Date.ToString("d");
            worksheet.Range[2, 1].Value = "Запчастина";
            worksheet.Range[2, 2].Value = "К-ть";
            worksheet.Range[2, 3].Value = "Ціна";
            worksheet.Range[2, 4].Value = "Сума";

            for (int i = 0; i < invoice.InvoiceParts.Count; i++)
            {
                worksheet.Range[i + 3, 1].Value = invoice.InvoiceParts[i].Part.Name;
                worksheet.Range[i + 3, 2].Value = invoice.InvoiceParts[i].Count.ToString();
                worksheet.Range[i + 3, 3].Value = invoice.InvoiceParts[i].PriceOutWithDelivery.ToString("C2");
                var formula1 = $"=B{i + 3}*C{i + 3}";
                worksheet.Range[i + 3, 4].Formula = formula1;
            }

            var formula = $"=SUM(D3:D{invoice.InvoiceParts.Count + 2})";
            worksheet.Range[invoice.InvoiceParts.Count + 3, 4].Formula = formula;
            worksheet.SetColumnWidth(1, 25);
            worksheet.SetColumnWidth(2, 10);
            worksheet.SetColumnWidth(3, 15);
            worksheet.SetColumnWidth(4, 15);
            worksheet.SetRowHeight(1, 25);
            worksheet.Range[2, 1, 2, 4].Style.Color = System.Drawing.Color.LightGray;
            worksheet.Range[1, 1, invoice.InvoiceParts.Count + 3, 4].Borders.Color = System.Drawing.Color.DarkGray;

            workbook.SaveToFile($"reports/invoice{invoice.Id}.xlsx", ExcelVersion.Version2016);

        }
    }
}