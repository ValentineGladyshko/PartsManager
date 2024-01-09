using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Xml.Linq;

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isInvoiceCreated;
        private bool isPartSelected;
        private bool isCarSelected;

        public bool IsInvoiceCreated
        {
            get { return isInvoiceCreated; }
            set
            {
                isInvoiceCreated = value;
                OnPropertyChanged("IsInvoiceCreated");
            }
        }
        public bool IsPartSelected
        {
            get { return isPartSelected; }
            set
            {
                isPartSelected = value;
                OnPropertyChanged("IsPartSelected");
            }
        }
        public bool IsCarSelected
        {
            get { return isCarSelected; }
            set
            {
                isCarSelected = value;
                OnPropertyChanged("IsCarSelected");
            }
        }

        public Invoice LocalInvoice { get; set; }
        public ICollection<InvoicePart> LocalInvoiceParts { get; set; }
        public InvoicePart LocalInvoicePart { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public InvoiceWindow()
        {
            InitializeComponent();
            LocalInvoice = new Invoice()
            {
                Car = new Car()
                {
                    Model = new Model.Entities.Model()
                    {
                        Mark = new Mark()
                    },
                    VINCode = string.Empty,
                    Info = string.Empty,
                },
                Date = DateTime.Now,
                Info = string.Empty,
            };
            LocalInvoicePart = new InvoicePart()
            {
                Part = new Part(),
                Invoice = LocalInvoice,
                Count = 1,
                PriceIn = 0,
                PriceOut = 0,
            };
            SetContent();
            Action = ActionType.Create;
            IsInvoiceCreated = false;
            IsPartSelected = false;
            IsCarSelected = false;
            DataContext = this;
            WorkButton.IsEnabled = false;
            InvoiceNotificationBlock.Text = "Для створення накладної спочатку необхідно обрати автомобіль";
            InvoicePartNotificationBlock.Text = "Для додавання запчастини спочатку необхідно створити накладну та обрати запчастину";
            InvoiceNotificationBlock.Foreground = Brushes.DarkRed;
            InvoicePartNotificationBlock.Foreground = Brushes.DarkRed;
            SetHandlers();
        }
        public InvoiceWindow(Invoice invoice)
        {
            InitializeComponent();
            LocalInvoice = invoice;           
            LocalInvoicePart = new InvoicePart()
            {
                Part = new Part(),
                Invoice = LocalInvoice,
                Count = 1,
                PriceIn = 0,
                PriceOut = 0,
            };
            SetContent();
            IsInvoiceCreated = true;
            IsPartSelected = false;
            IsCarSelected = true;
            LocalInvoiceParts = unitOfWork.InvoiceParts.GetAll()
                .Where(item => item.InvoiceId == LocalInvoice.Id).ToList();

            DataContext = this;
            SetHandlers();
        }

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void SetContent()
        {
            CreateInvoicePartButton.IsEnabled = false;

            PropertyChanged += ChangeButtons;
        }

        private void ChangeButtons(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsCarSelected")
            {
                if (!IsCarSelected)
                {
                    InvoiceNotificationBlock.Text = "Для створення накладної спочатку необхідно обрати автомобіль";
                    InvoiceNotificationBlock.Foreground = Brushes.DarkRed;
                }
                else
                {
                    InvoiceNotificationBlock.Text = "Автомобіль обраний";
                    InvoiceNotificationBlock.Foreground = Brushes.DarkGreen;
                }
            }

            if (!IsInvoiceCreated)
            {
                if (!IsPartSelected)
                {
                    InvoicePartNotificationBlock.Text = "Для додавання запчастини спочатку необхідно створити накладну та обрати запчастину";
                    InvoicePartNotificationBlock.Foreground = Brushes.DarkRed;
                }
                else
                {
                    InvoicePartNotificationBlock.Text = "Для додавання запчастини спочатку необхідно створити накладну";
                    InvoicePartNotificationBlock.Foreground = Brushes.DarkRed;
                }
            }
            else
            {
                if (!IsPartSelected)
                {
                    InvoicePartNotificationBlock.Text = "Для додавання запчастини спочатку необхідно обрати запчастину";
                    InvoicePartNotificationBlock.Foreground = Brushes.DarkRed;
                }
                else
                {
                    InvoicePartNotificationBlock.Text = "Запчастина обрана";
                    InvoicePartNotificationBlock.Foreground = Brushes.DarkGreen;
                    CreateInvoicePartButton.IsEnabled = true;
                }
            }

            if (e.PropertyName == "IsInvoiceCreated" && IsInvoiceCreated)
            {
                WorkButton.Content = "Редагувати накладну";
                Action = ActionType.Edit;
            }        
        }

        public void SetHandlers()
        {
            WorkButton.Click += (object sender, RoutedEventArgs e) =>
            {
                if (Action == ActionType.Create)
                {
                    unitOfWork.Invoices.Create(LocalInvoice);
                    unitOfWork.Save();
                    IsInvoiceCreated = true;
                }
                else if (Action == ActionType.Edit)
                {
                    unitOfWork.Invoices.Update(LocalInvoice);
                    unitOfWork.Save();
                }
            };
            SelectPartButton.Click += (object sender, RoutedEventArgs e) =>
            {
                PartSelectionWindow partSelectionWindow = new PartSelectionWindow();
                bool? dialogResult = partSelectionWindow.ShowDialog();
                if (dialogResult != true)
                    return;
                else
                {
                    LocalInvoicePart.Part = partSelectionWindow.LocalPart;
                    PartBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    IsPartSelected = true; 
                }
            };
            SelectCarButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var carSelectionWindow = new CarSelectionWindow();
                bool? dialogResult = carSelectionWindow.ShowDialog();
                if (dialogResult != true)
                    return;
                else
                {
                    LocalInvoice.Car = carSelectionWindow.LocalCar;
                    CarBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    IsCarSelected = true;
                    WorkButton.IsEnabled = true;
                }
            };
            CreateInvoicePartButton.Click += (object sender, RoutedEventArgs e) =>
            {
                unitOfWork.InvoiceParts.Create(LocalInvoicePart);
                unitOfWork.Save();

                LocalInvoiceParts = unitOfWork.InvoiceParts.GetAll()
                    .Where(item => item.InvoiceId == LocalInvoice.Id).ToList();
                InvoicePartListBox.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            };
            PriceInBox.LostFocus += (object sender, RoutedEventArgs e) =>
            {
                RecommendedPriceBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                SumInBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            };
            PriceOutBox.LostFocus += (object sender, RoutedEventArgs e) =>
            {
                SumOutBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            };
        }

        public void DeleteInvoicePartOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partInvoiceToDelete = unitOfWork.InvoiceParts.Get((int)button.Tag);

            string message = $"Ви впевнені що хочете видалити запчастину \"{partInvoiceToDelete.Part}\" з накладної?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.InvoiceParts.Delete(partInvoiceToDelete.Id);
            unitOfWork.Save();

            LocalInvoiceParts = unitOfWork.InvoiceParts.GetAll()
                .Where(item => item.InvoiceId == LocalInvoice.Id).ToList();
            InvoicePartListBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
        }
    }
}
