using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ModelWindow2.xaml
    /// </summary>
    public partial class ModelWindow2 : Window
    {
        private Model.Entities.Model LocalModel { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");
        public ModelWindow2()
        {
            InitializeComponent();

            LocalModel = new Model.Entities.Model()
            {
                Mark = new Mark()
            };

            Action = ActionType.Create;
            SetContent();
            SetHandlers();
        }

        public ModelWindow2(Model.Entities.Model model, ActionType action)
        {
            InitializeComponent();

            LocalModel = model;
            Action = action;

            SetContent();
            SetHandlers();
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

            var markList = unitOfWork.Marks.GetAll().ToList();
            markList.Sort();
            MarkBox.ItemsSource = markList;

            if (MarkBox.Items.Contains(LocalModel.Mark))
            {
                MarkBox.SelectedIndex = MarkBox.Items.IndexOf(LocalModel.Mark);
            }
            else
            {
                if (markList.Count != 0)
                    MarkBox.SelectedIndex = MarkBox.Items.IndexOf(markList.First());
            }

            NameBox.Text = LocalModel.Name;
        }

        public void SetHandlers()
        {
            CancelButton.Click += (object sender, RoutedEventArgs e) => { Close(); };

            WorkButton.Click += (object sender, RoutedEventArgs e) =>
            {
                if (MarkBox.SelectedIndex == -1)
                {
                    MarkBorder.BorderBrush = new SolidColorBrush(Colors.Red); //#FFACACAC
                    return;
                }
                LocalModel.Mark = MarkBox.SelectedItem as Mark;
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

            MarkBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                MarkBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            };
        }
    }
}
