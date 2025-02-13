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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PartsManager
{

    public partial class InvoicePackingListPage : UserControl
    {
        public InvoicePackingListPage(IEnumerable<PackingListPartInfo> packingListParts, Invoice invoice)
        {
            InitializeComponent();
            InvoiceNumberTextBlock.Text = $"Пакувальний Лист до Накладної №{invoice.Id}\nвід {invoice.Date:dd MMMM yyyy}";
            CarTextBlock.Text = $"{invoice.Car.Model.Mark.Name} {invoice.Car.Model.Name}";
            VINCodeTextBlock.Text = invoice.Car.VINCode;

            var packingListPartGrid = new PackingListPartGrid(packingListParts);
            Grid.SetRow(packingListPartGrid, 3);
            MainGrid.Children.Add(packingListPartGrid);
        }

        public InvoicePackingListPage()
        {
            InitializeComponent();
        }
    }
}
