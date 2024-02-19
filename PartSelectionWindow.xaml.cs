using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PartsManager
{
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
                    .Select(item => item.Name).GroupBy(item => item).Select(item => item.Key).ToList();
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

            SearchPartButton.Click += delegate
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
            CreatePartButton.Click += delegate
            {
                LocalPart.PartType.Name = PartPartTypeNameBox.Text;

                var partWindow = new PartWindow(LocalPart, ActionType.Create)
                {
                    Owner = this
                };
                partWindow.Closed += delegate
                {
                    SearchPartButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                };

                partWindow.Show();
            };
            PartListBox.SelectionChanged += delegate
            {
                if (PartListBox.SelectedItem is Part part)
                {
                    LocalPart = part;
                    DialogResult = true;
                    Close();
                }
            };
        }
    }
}
