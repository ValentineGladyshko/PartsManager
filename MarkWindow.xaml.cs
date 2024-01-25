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
    /// Interaction logic for MarkWindow.xaml
    /// </summary>
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
            CancelButton.Click += (object sender, RoutedEventArgs e) => { Close(); };

            WorkButton.Click += (object sender, RoutedEventArgs e) =>
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
