using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System.Linq;
using System.Windows;

namespace PartsManager
{
    public partial class ModelWindow : Window
    {
        private Model.Entities.Model LocalModel { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");
        public ModelWindow()
        {
            InitializeComponent();

            LocalModel = new Model.Entities.Model()
            {
                Mark = new Mark()
            };

            Action = ActionType.Create;

            SetHandlers();
            SetContent();
        }

        public ModelWindow(Model.Entities.Model model, ActionType action)
        {
            InitializeComponent();

            LocalModel = model;
            Action = action;

            SetHandlers();
            SetContent();
        }

        public void SetContent()
        {
            if (Action == ActionType.Edit)
            {
                Title = "Редагування моделі авто";
                WorkButton.Content = "Редагувати";
            }
            else if (Action == ActionType.Create)
            {
                Title = "Створення моделі авто";
                WorkButton.Content = "Створити";
            }

            NameBox.Text = LocalModel.Name;
            ModelMarkNameBox.Text = LocalModel.Mark.Name.FirstCharToUpper();
        }

        public void SetHandlers()
        {
            CancelButton.Click += delegate { Close(); };

            WorkButton.Click += delegate
            {
                if (ModelMarkNameBox.Text == string.Empty)
                    return;

                var marks = unitOfWork.Marks.Find(item => item.Name == ModelMarkNameBox.Text).ToList();

                if (marks.Count == 0)
                {
                    string message = "Для " + TextBoxHelper.ActionText(Action) + " даної моделі також треба створити марку \""
                            + ModelMarkNameBox.Text + "\"\nВи згодні з створенням марки?";
                    var dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    var mark = new Mark()
                    {
                        Name = ModelMarkNameBox.Text,
                    };

                    unitOfWork.Marks.Create(mark);
                    unitOfWork.Save();
                    LocalModel.Mark = mark;
                }
                else
                {
                    LocalModel.Mark = marks.First();
                }

                LocalModel.MarkId = LocalModel.Mark.Id;
                LocalModel.Name = NameBox.Text;

                if (Action == ActionType.Edit)
                {
                    unitOfWork.Models.Update(LocalModel);
                    unitOfWork.Save();
                    Close();
                }
                else if (Action == ActionType.Create)
                {
                    unitOfWork.Models.Create(LocalModel);
                    unitOfWork.Save();
                    Close();
                }
            };

            ModelMarkNameBox.SetDropDownOpened(unitOfWork.Marks.GetAll());
            NameBox.SetTextChangedFirstCharToUpper();
            ModelMarkNameBox.SetTextChangedFirstCharToUpper();
        }
    }
}
