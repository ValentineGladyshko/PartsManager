using PartsManager.Model.Entities;
using PartsManager.Model.ViewModels;
using PdfSharp.Xps;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for PackingListWindow.xaml
    /// </summary>
    public partial class PackingListWindow : Window
    {
        public PackingListWindow(Invoice invoice)
        {
            int index = 1;
            var packingListParts = (from invoicePart in invoice.InvoiceParts.ToList()
                                select new PackingListPartInfo
                                {
                                    Index = index++.ToString(),
                                    PartName = invoicePart.Part.Name,
                                    Article = invoicePart.Part.Article,
                                    Count = invoicePart.Count.ToString(),
                                }).ToList();
            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = new Size(794, 1123);
            var mainPage = new FixedPage
            {
                Height = document.DocumentPaginator.PageSize.Height,
                Width = document.DocumentPaginator.PageSize.Width
            };
            var reportPage = new InvoicePackingListPage();
            const int RowsPerFirstPage = 30;
            const int RowsPerPage = 50;
            if (packingListParts.Count() > RowsPerFirstPage)
            {
                var firstPartSegment = packingListParts.GetRange(0, RowsPerFirstPage);
                reportPage = new InvoicePackingListPage(firstPartSegment, invoice);

                mainPage.Children.Add(reportPage);
                PageContent pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(mainPage);
                document.Pages.Add(pageContent);

                var invoicePartsSegments = new List<List<PackingListPartInfo>>();
                for (int i = RowsPerFirstPage; i < packingListParts.Count; i += RowsPerPage)
                {
                    invoicePartsSegments.Add(packingListParts.GetRange(i, Math.Min(RowsPerPage, packingListParts.Count - i)));
                }

                var partGrids = invoicePartsSegments.GetRange(0, invoicePartsSegments.Count - 1).ConvertAll(item => new PackingListPartGrid(item));
                partGrids.Add(new PackingListPartGrid(invoicePartsSegments.Last()));

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
                reportPage = new InvoicePackingListPage(packingListParts, invoice);
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
            DocumentPackingList.Document = document;
            XpsConverter.Convert("output.xps", $"reports/invoicePackingList{invoice.Id}.pdf", 1);
        }
    }
}
