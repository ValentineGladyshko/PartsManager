using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System.Windows;

namespace PartsManager
{
    public partial class PartTypeWindow : Window
    {
        private PartType LocalPartType { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

        public PartTypeWindow()
        {
            InitializeComponent();

            LocalPartType = new PartType();
            Action = ActionType.Create;

            SetContent();
            SetHandlers();
        }

        public PartTypeWindow(PartType partType, ActionType action)
        {
            InitializeComponent();

            LocalPartType = partType;
            Action = action;

            SetContent();
            SetHandlers();
        }

        public void SetContent()
        {
            if (Action == ActionType.Edit)
            {
                Title = "Редагування типу запчастин";
                WorkButton.Content = "Редагувати";
            }
            else if (Action == ActionType.Create)
            {
                Title = "Створення типу запчастин";
                WorkButton.Content = "Створити";
            }

            NameBox.Text = LocalPartType.Name;
        }

        public void SetHandlers()
        {
            CancelButton.Click += delegate { Close(); };

            WorkButton.Click += delegate
            {
                LocalPartType.Name = NameBox.Text;

                if (Action == ActionType.Edit)
                {
                    unitOfWork.PartTypes.Update(LocalPartType);
                    unitOfWork.Save();
                    Close();
                }
                else if (Action == ActionType.Create)
                {
                    unitOfWork.PartTypes.Create(LocalPartType);
                    unitOfWork.Save();
                    Close();
                }
            };

            NameBox.SetTextChangedFirstCharToUpper();
        }
    }
}
