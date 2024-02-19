using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace PartsManager
{
    public partial class CarSelectionWindow : Window
    {
        public Car LocalCar { get; private set; }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");
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
            CarModelNameBox.DropDownOpened += delegate
            {
                var list = unitOfWork.Models.GetAll()
                    .Where(item => item.Name.Contains(CarModelNameBox.Text)
                        && item.Mark.Name.Contains(CarMarkNameBox.Text))
                    .Select(item => item.Name).ToList();
                list.Sort();
                CarModelNameBox.ItemsSource = list;
            };
            SearchCarButton.Click += delegate
            {
                var list = unitOfWork.Cars.GetAll()
                    .Where(item => item.Info.Contains(LocalCar.Info)
                        && item.Model.Name.Contains(CarModelNameBox.Text)
                        && item.VINCode.Contains(LocalCar.VINCode)
                        && item.Model.Mark.Name.Contains(CarMarkNameBox.Text))
                    .ToList();
                CarListBox.ItemsSource = list;
            };

            CreateCarButton.Click += delegate
            {
                LocalCar.Model.Name = CarModelNameBox.Text;
                LocalCar.Model.Mark.Name = CarMarkNameBox.Text;

                var carWindow = new CarWindow(LocalCar, ActionType.Create)
                {
                    Owner = this
                };
                carWindow.Closed += delegate
                {
                    SearchCarButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                };

                carWindow.Show();
            };

            CarListBox.SelectionChanged += delegate
            {
                if (CarListBox.SelectedItem is Car car)
                {
                    LocalCar = car;
                    DialogResult = true;
                    Close();
                }
            };
        }
    }
}
