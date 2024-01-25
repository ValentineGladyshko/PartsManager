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
    /// Interaction logic for PartTypeWindow.xaml
    /// </summary>

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
            CancelButton.Click += (object sender, RoutedEventArgs e) => { Close(); };

            WorkButton.Click += (object sender, RoutedEventArgs e) =>
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
