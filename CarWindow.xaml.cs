using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System.Linq;
using System.Windows;

namespace PartsManager
{
    public partial class CarWindow : Window
    {
        private Car LocalCar { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");
        public CarWindow()
        {
            InitializeComponent();

            LocalCar = new Car()
            {
                Model = new Model.Entities.Model()
                {
                    Mark = new Mark()
                }
            };

            Action = ActionType.Create;
            SetContent();
            SetHandlers();
        }

        public CarWindow(Car car, ActionType action)
        {
            InitializeComponent();

            LocalCar = car;
            Action = action;

            SetContent();
            SetHandlers();
        }

        public void SetContent()
        {
            if (Action == ActionType.Edit)
            {
                Title = "Редагування автомобіля";
                WorkButton.Content = "Редагувати";
            }
            else if (Action == ActionType.Create)
            {
                Title = "Створення автомобіля";
                WorkButton.Content = "Створити";
            }

            VINCodeBox.Text = LocalCar.VINCode.ToUpper(); 
            CarModelNameBox.Text = LocalCar.Model.Name.FirstCharToUpper();
            CarMarkNameBox.Text = LocalCar.Model.Mark.Name.FirstCharToUpper();
        }

        public void SetHandlers()
        {     
            CancelButton.Click += delegate { Close(); };

            WorkButton.Click += delegate
            {
                if (CarMarkNameBox.Text == string.Empty || CarModelNameBox.Text == string.Empty)
                    return;

                var marks = unitOfWork.Marks.Find(item => item.Name == CarMarkNameBox.Text).ToList();
                var models = unitOfWork.Models.Find(item => item.Name == CarModelNameBox.Text).ToList();

                if (marks.Count == 0 && models.Count == 0)
                {
                    string message = "Для " + TextBoxHelper.ActionText(Action) + " даного автомобіля також треба створити марку \""
                            + CarMarkNameBox.Text + "\" та модель \"" 
                            + CarModelNameBox.Text + "\"\nВи згодні з створенням марки та моделі?";
                    var dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    var mark = new Mark()
                    {
                        Name = CarMarkNameBox.Text,
                    };

                    unitOfWork.Marks.Create(mark);
                    unitOfWork.Save();

                    var model = new Model.Entities.Model()
                    {
                        Name = CarModelNameBox.Text,
                        Mark = mark,
                        MarkId = mark.Id,
                    };
                    unitOfWork.Models.Create(model);
                    unitOfWork.Save();

                    LocalCar.Model = model;
                }
                else if (marks.Count == 0 && models.Count != 0)
                {
                    return;
                }
                else if (marks.Count != 0 && models.Count == 0)
                {
                    string message = "Для " + TextBoxHelper.ActionText(Action) + " даного автомобіля також треба створити модель \""
                            + CarModelNameBox.Text + "\"\nВи згодні з створенням моделі?";
                    var dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    var model = new Model.Entities.Model()
                    {
                        Name = CarModelNameBox.Text,
                        Mark = marks.First(),
                        MarkId = marks.First().Id,
                    };
                    unitOfWork.Models.Create(model);
                    unitOfWork.Save();

                    LocalCar.Model = model;
                }
                else if (marks.Count != 0 && models.Count != 0)
                {
                    LocalCar.Model = models.First();
                }
                else 
                { 
                    return; 
                }

                LocalCar.ModelId = LocalCar.Model.Id;
                LocalCar.VINCode = VINCodeBox.Text;
                LocalCar.Info = InfoBox.Text;

                if (Action == ActionType.Edit)
                {
                    unitOfWork.Cars.Update(LocalCar);
                    unitOfWork.Save();
                    Close();
                }
                else if (Action == ActionType.Create)
                {
                    unitOfWork.Cars.Create(LocalCar);
                    unitOfWork.Save();
                    Close();
                }
            };

            CarModelNameBox.SetDropDownOpened(unitOfWork.Models.GetAll());
            CarMarkNameBox.SetDropDownOpened(unitOfWork.Marks.GetAll());
            CarModelNameBox.SetTextChangedFirstCharToUpper();
            CarMarkNameBox.SetTextChangedFirstCharToUpper();
            VINCodeBox.SetTextChangedToUpper();
        }
    }
}
