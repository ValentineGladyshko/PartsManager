using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
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
    /// Interaction logic for CarSelectionWindow.xaml
    /// </summary>
    public partial class CarSelectionWindow : Window
    {
        public Car LocalCar { get; private set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");
        public CarSelectionWindow()
        {
            InitializeComponent();
            LocalCar = new Car()
            {
                Model = new Model.Entities.Model()
                {
                    Mark = new Mark()
                },
                VINCode = string.Empty,
                Info = string.Empty,
            };
            DataContext = LocalCar;
            SetPartHandlers();
        }

        public void SetPartHandlers()
        {
            CarMarkNameBox.SetDropDownOpened(unitOfWork.Marks.GetAll());
            CarModelNameBox.SetDropDownOpened(unitOfWork.Models.GetAll());

            SearchCarButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.Cars.GetAll()
                    .Where(item => item.Info.Contains(LocalCar.Info)
                        && item.Model.Name.Contains(CarModelNameBox.Text)
                        && item.VINCode.Contains(LocalCar.VINCode)
                        && item.Model.Mark.Name.Contains(CarMarkNameBox.Text))
                    .ToList();
                CarListBox.ItemsSource = list;
            };

            CreateCarButton.Click += (object sender, RoutedEventArgs args) =>
            {
                LocalCar.Model.Name = CarModelNameBox.Text;
                LocalCar.Model.Mark.Name = CarMarkNameBox.Text;

                var carWindow = new CarWindow(LocalCar, ActionType.Create);
                carWindow.Owner = this;
                carWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    SearchCarButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                carWindow.Show();
            };

            CarListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var car = CarListBox.SelectedItem as Car;
                if (car != null)
                {
                    LocalCar = car;
                    DialogResult = true;
                    Close();
                }
            };
        }
    }
}
