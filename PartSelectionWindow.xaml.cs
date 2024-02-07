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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for PartSelectionWindow.xaml
    /// </summary>
    public partial class PartSelectionWindow : Window
    {
        public Part LocalPart { get; private set; }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");
        public PartSelectionWindow()
        {
            InitializeComponent();
            LocalPart = new Part()
            {
                PartType = new PartType(),
                Name = string.Empty,
                FullName = string.Empty,
                Article = string.Empty,
                Description = string.Empty,
            };
            DataContext = LocalPart;
            SetPartHandlers();
        }

        public PartSelectionWindow(string type)
        {
            InitializeComponent();
            LocalPart = new Part()
            {
                PartType = new PartType(),
                Name = string.Empty,
                FullName = string.Empty,
                Article = string.Empty,
                Description = string.Empty,
            };
            DataContext = LocalPart;
            SetPartHandlers();
            PartPartTypeNameBox.Text = type;
            SearchPartButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        public void SetPartHandlers()
        {
            PartPartTypeNameBox.SetDropDownOpened(unitOfWork.PartTypes.GetAll());
            PartNameBox.DropDownOpened += delegate
            {
                var list = unitOfWork.Parts.GetAll()
                    .Where(item => item.Name.Contains(PartNameBox.Text)
                        && item.PartType.Name.Contains(PartPartTypeNameBox.Text))
                    .Select(item => item.Name).ToList();
                list.Sort();
                PartNameBox.ItemsSource = list;
            };
            PartFullNameBox.DropDownOpened += delegate
            {
                var list = unitOfWork.Parts.GetAll()
                    .Where(item => item.FullName.Contains(PartFullNameBox.Text)
                        && item.PartType.Name.Contains(PartPartTypeNameBox.Text))
                    .Select(item => item.FullName).ToList();
                list.Sort();
                PartFullNameBox.ItemsSource = list;
            };

            SearchPartButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.Parts.GetAll()
                    .Where(item => item.Name.Contains(LocalPart.Name)
                        && item.FullName.Contains(LocalPart.FullName)
                        && item.PartType.Name.Contains(PartPartTypeNameBox.Text)
                        && item.Article.Contains(LocalPart.Article)
                        && item.Description.Contains(LocalPart.Description))
                    .ToList();
                PartListBox.ItemsSource = list;
            };
            CreatePartButton.Click += (object sender, RoutedEventArgs args) =>
            {
                LocalPart.PartType.Name = PartPartTypeNameBox.Text;

                PartWindow partWindow = new PartWindow(LocalPart, ActionType.Create);
                partWindow.Owner = this;
                partWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    SearchPartButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                partWindow.Show();
            };
            PartListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var part = PartListBox.SelectedItem as Part;
                if (part != null)
                {
                    LocalPart = part;
                    DialogResult = true;
                    Close();
                }
            };
        }
    }
}
