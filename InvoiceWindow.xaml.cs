using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PartsManager
{
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

        public InvoiceWindow(bool isMine)
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
                IsMine = isMine,
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
            VINCodeBox.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xE9, 0xE9, 0xE9));
            VINCodeBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD5, 0xD6, 0xD9));
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
            Closing += delegate
            {
                if (LocalInvoice.Residue <= 0 && LocalInvoice.IsPayed == false)
                {
                    LocalInvoice.IsPayed = true;
                    unitOfWork.Invoices.Update(LocalInvoice);
                    unitOfWork.Save();
                }
                else if (LocalInvoice.Residue > 0 && LocalInvoice.IsPayed == true)
                {
                    LocalInvoice.IsPayed = false;
                    unitOfWork.Invoices.Update(LocalInvoice);
                    unitOfWork.Save();
                }
            };
            WorkButton.Click += delegate
            {
                if (Action == ActionType.Create)
                {
                    unitOfWork.Invoices.Create(LocalInvoice);
                    unitOfWork.Save();
                    IsInvoiceCreated = true;
                    InvoiceIncomeBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    InvoicePartnerSumBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }
                else if (Action == ActionType.Edit)
                {
                    unitOfWork.Invoices.Update(LocalInvoice);
                    unitOfWork.Save();
                    InvoiceIncomeBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    InvoicePartnerSumBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }
            };
            SelectPartButton.Click += delegate
            {
                var partSelectionWindow = new PartSelectionWindow();
                bool? dialogResult = partSelectionWindow.ShowDialog();
                if (dialogResult != true)
                    return;
                else
                {
                    LocalInvoicePart = new InvoicePart()
                    {
                        Part = partSelectionWindow.LocalPart,
                        Invoice = LocalInvoice,
                        Count = 1,
                        PriceIn = 0,
                        PriceOut = 0,
                    };
                    LastPriceOutBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                    PartBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    IsPartSelected = true;
                    IsPartEditing = false;
                }
            };
            SelectCarButton.Click += delegate
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
            WorkInvoicePartButton.Click += delegate
            {
                if (!IsPartEditing)
                {
                    if ((LocalInvoicePart.Part != null && (LocalInvoice.InvoiceParts == null
                            || LocalInvoice.InvoiceParts.Where(item => item.PartId == LocalInvoicePart.Part.Id).Count() == 0))
                        || (LocalInvoicePart.PartId != 0 && (LocalInvoice.InvoiceParts == null
                            || LocalInvoice.InvoiceParts.Where(item => item.PartId == LocalInvoicePart.PartId).Count() == 0)))
                    {
                        if (LocalInvoicePart.PriceOut > (LocalInvoicePart.RecommendedPrice * 2) || LocalInvoicePart.PriceOut < LocalInvoicePart.PriceIn)
                        {
                            string message = $"Ціна продажу: {LocalInvoicePart.PriceOut:C2} \nзначно більша за рекомендовану: {LocalInvoicePart.RecommendedPrice:C2}";

                            var dialogWindow = new DialogWindow(message);
                            bool? dialogResult = dialogWindow.ShowDialog();

                            if (dialogResult != true)
                                return;
                        }
                        if (LocalInvoicePart.PriceOut < LocalInvoicePart.PriceIn)
                        {
                            string message = $"Ціна продажу: {LocalInvoicePart.PriceOut:C2} \nменша за ціну покупки: {LocalInvoicePart.PriceIn:C2}";

                            var dialogWindow = new DialogWindow(message);
                            bool? dialogResult = dialogWindow.ShowDialog();

                            if (dialogResult != true)
                                return;
                        }
                        if (PartPrices != null && PartPrices.Count > 0 && (LocalInvoicePart.PriceOut > (PartPrices.First().PriceOut * 2) || LocalInvoicePart.PriceOut < (PartPrices.First().PriceOut * 0.7m)))
                        {
                            string message = $"Ціна продажу: {LocalInvoicePart.PriceOut:C2} \nсильно відрізняється від останньої ціни: {PartPrices.First().PriceOut:C2}";

                            DialogWindow dialogWindow = new DialogWindow(message);
                            bool? dialogResult = dialogWindow.ShowDialog();

                            if (dialogResult != true)
                                return;
                        }
                        
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
                    if (LocalInvoicePart.PriceOut > (LocalInvoicePart.RecommendedPrice * 2) || LocalInvoicePart.PriceOut < LocalInvoicePart.PriceIn)
                    {
                        string message = $"Ціна продажу: {LocalInvoicePart.PriceOut:C2} \nзначно більша за рекомендовану: {LocalInvoicePart.RecommendedPrice:C2}";

                        DialogWindow dialogWindow = new DialogWindow(message);
                        bool? dialogResult = dialogWindow.ShowDialog();

                        if (dialogResult != true)
                            return;
                    }
                    if (LocalInvoicePart.PriceOut < LocalInvoicePart.PriceIn)
                    {
                        string message = $"Ціна продажу: {LocalInvoicePart.PriceOut:C2} \nменша за ціну покупки: {LocalInvoicePart.PriceIn:C2}";

                        DialogWindow dialogWindow = new DialogWindow(message);
                        bool? dialogResult = dialogWindow.ShowDialog();

                        if (dialogResult != true)
                            return;
                    }
                    if (PartPrices != null && PartPrices.Count > 0 && (LocalInvoicePart.PriceOut > (PartPrices.First().PriceOut * 2) || LocalInvoicePart.PriceOut < (PartPrices.First().PriceOut * 0.7m)))
                    {
                        string message = $"Ціна продажу: {LocalInvoicePart.PriceOut:C2} \nсильно відрізняється від останньої ціни: {PartPrices.First().PriceOut:C2}";

                        DialogWindow dialogWindow = new DialogWindow(message);
                        bool? dialogResult = dialogWindow.ShowDialog();

                        if (dialogResult != true)
                            return;
                    }
                    
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
            CountBox.LostFocus += delegate
            {
                SumInBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                SumOutBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            };
            PriceInBox.LostFocus += delegate
            {
                RecommendedPriceBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                SumInBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            };
            PriceOutBox.LostFocus += delegate
            {
                SumOutBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            };
            InvoicePartDataGrid.SelectionChanged += delegate
            {
                if (InvoicePartDataGrid.SelectedItem is InvoicePart invoicePart)
                {
                    LocalInvoicePart = invoicePart;
                    Clipboard.SetText(invoicePart.Part.Article);
                }
                IsPartSelected = true;
                IsPartEditing = true;
                InvoicePartDataGrid.UnselectAll();
            };
            VINCodeBox.PreviewMouseLeftButtonUp += delegate
            {
                Clipboard.SetText(VINCodeBox.Text);
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

            var dialogWindow = new DialogWindow(message);
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

    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int rowNumber = (int)value;
            return rowNumber + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
