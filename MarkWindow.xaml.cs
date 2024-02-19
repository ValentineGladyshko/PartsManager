using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System.Windows;

namespace PartsManager
{
    public partial class MarkWindow : Window
    {
        private Mark LocalMark { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

        public MarkWindow()
        {
            InitializeComponent();

            LocalMark = new Mark();
            Action = ActionType.Create;

            SetHandlers();
            SetContent();
        }

        public MarkWindow(Mark mark, ActionType action)
        {
            InitializeComponent();

            LocalMark = mark;
            Action = action;

            SetHandlers();
            SetContent();
        }

        public void SetContent()
        {
            if (Action == ActionType.Edit)
            {
                Title = "Редагування марки авто";
                WorkButton.Content = "Редагувати";
            }
            else if (Action == ActionType.Create)
            {
                Title = "Створення марки авто";
                WorkButton.Content = "Створити";
            }

            NameBox.Text = LocalMark.Name;
        }

        public void SetHandlers()
        {
            CancelButton.Click += delegate { Close(); };

            WorkButton.Click += delegate
            {
                LocalMark.Name = NameBox.Text;

                if (Action == ActionType.Edit)
                {
                    unitOfWork.Marks.Update(LocalMark);
                    unitOfWork.Save();
                    Close();
                }
                else if (Action == ActionType.Create)
                {
                    unitOfWork.Marks.Create(LocalMark);
                    unitOfWork.Save();
                    Close();
                }
            };

            NameBox.SetTextChangedFirstCharToUpper();
        }
    }
}
