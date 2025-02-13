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

    public partial class PackingListPartGrid : UserControl
    {
        public PackingListPartGrid()
        {
            InitializeComponent();
        }
        public PackingListPartGrid(IEnumerable<PackingListPartInfo> packingListParts)
        {
            InitializeComponent();
            PackingListPartDataGrid.ItemsSource = packingListParts;
            HeadersDataGrid.ItemsSource = new[]
            {
                new {Index = "№", PartName = "Запчастина",Article = "Артикуль", Count = "Кількість" },
            };
        }
    }
}
