using Microsoft.SqlServer.Management.Smo;
using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
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
        private bool isPartEditing;
        private InvoicePart localInvoicePart;

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
        public bool IsPartEditing
        {
            get { return isPartEditing; }
            set
            {
                isPartEditing = value;
                OnPropertyChanged("IsPartEditing");
            }
        }
        public InvoicePart LocalInvoicePart
        {
            get { return localInvoicePart; }
            set
            {
                localInvoicePart = value;
                OnPropertyChanged("LocalInvoicePart");
                OnPropertyChanged("PartPrices");
            }
        }

        public ICollection<InvoicePart> PartPrices
        {
            get 
            {
                if (LocalInvoicePart.Part.InvoiceParts == null)
                    return LocalInvoicePart.Part.InvoiceParts;
                else 
                    return LocalInvoicePart.Part.InvoiceParts.OrderByDescending(item => item.Invoice.Date).ToList();
            }
        }

        public Invoice LocalInvoice { get; set; }
        public ICollection<InvoicePart> LocalInvoiceParts { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

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
            IsPartEditing = false;
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
            IsPartEditing = false;
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
            WorkInvoicePartButton.IsEnabled = false;

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
                    WorkInvoicePartButton.IsEnabled = true;
                }
            }

            if (e.PropertyName == "IsInvoiceCreated" && IsInvoiceCreated)
            {
                WorkButton.Content = "Редагувати накладну";
                Action = ActionType.Edit;
            }

            if (e.PropertyName == "IsPartEditing")
            {
                if (IsPartEditing)
                {
                    WorkInvoicePartButton.Content = "Редагувати запчастину";
                }
                else
                {
                    WorkInvoicePartButton.Content = "Додати запчастину в накладну";
                }
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
                    LastPriceOutBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                    PartBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    IsPartSelected = true;
                    IsPartEditing = false;
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
            WorkInvoicePartButton.Click += (object sender, RoutedEventArgs e) =>
            {
                if (!IsPartEditing)
                {
                    if ((LocalInvoicePart.Part != null && (LocalInvoice.InvoiceParts == null 
                            || LocalInvoice.InvoiceParts.Where(item => item.PartId == LocalInvoicePart.Part.Id).Count() == 0))
                        || (LocalInvoicePart.PartId != 0 && (LocalInvoice.InvoiceParts == null
                            || LocalInvoice.InvoiceParts.Where(item => item.PartId == LocalInvoicePart.PartId).Count() == 0)))
                    {
                        unitOfWork.InvoiceParts.Create(LocalInvoicePart);
                        unitOfWork.Save();

                        LocalInvoiceParts = unitOfWork.InvoiceParts.GetAll()
                            .Where(item => item.InvoiceId == LocalInvoice.Id).ToList();
                        InvoicePartDataGrid.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

                        var temp = LocalInvoicePart;
                        LocalInvoicePart = new InvoicePart()
                        {
                            Part = temp.Part,
                            Invoice = LocalInvoice,
                            Count = 1,
                            PriceIn = 0,
                            PriceOut = 0,
                        };
                    }
                    else
                    {
                        InvoicePartNotificationBlock.Text = "Дана запчастина вже додана";
                        InvoicePartNotificationBlock.Foreground = Brushes.DarkRed;
                    }
                }
                else
                {
                    unitOfWork.InvoiceParts.Update(LocalInvoicePart);
                    unitOfWork.Save();

                    LocalInvoiceParts = unitOfWork.InvoiceParts.GetAll()
                        .Where(item => item.InvoiceId == LocalInvoice.Id).ToList();
                    InvoicePartDataGrid.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

                    var temp = LocalInvoicePart;
                    LocalInvoicePart = new InvoicePart()
                    {
                        Part = temp.Part,
                        Invoice = LocalInvoice,
                        Count = 1,
                        PriceIn = 0,
                        PriceOut = 0,
                    };

                    IsPartEditing = false;
                }
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
            InvoicePartDataGrid.SelectionChanged += delegate
            {
                var invoicePart = InvoicePartDataGrid.SelectedItem as InvoicePart;
                if (invoicePart != null)
                {
                    LocalInvoicePart = invoicePart;
                }
                IsPartSelected = true;
                IsPartEditing = true;
                InvoicePartDataGrid.UnselectAll();
            };
        }

        public void DeleteInvoicePartOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

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
            InvoicePartDataGrid.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            IsPartEditing = false;
        }
    }
}
