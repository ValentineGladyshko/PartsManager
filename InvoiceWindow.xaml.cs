using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        private bool IsInvoiceCreated { get; set; }
        private bool IsPartSelected { get; set; }
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
            Action = ActionType.Create;
            IsInvoiceCreated = false;
            IsPartSelected = false;
            DataContext = this;
            SetContent();
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

            OnInvoiceCreated();

            IsPartSelected = false;
            LocalInvoiceParts = unitOfWork.InvoiceParts.GetAll()
                .Where(item => item.InvoiceId == LocalInvoice.Id).ToList();

            DataContext = this;
            SetContent();
            SetHandlers();
        }

        public void SetContent()
        {
            CreateInvoicePartButton.IsEnabled = false;
        }

        public void SetHandlers()
        {
            ComboBoxHelper.SetDropDownOpened(CarModelNameBox, unitOfWork.Models.GetAll());
            ComboBoxHelper.SetDropDownOpened(CarMarkNameBox, unitOfWork.Marks.GetAll());

            WorkButton.Click += (object sender, RoutedEventArgs e) =>
            {
                if (Action == ActionType.Create)
                {
                    if (CarMarkNameBox.Text == string.Empty || CarModelNameBox.Text == string.Empty
                        || LocalInvoice.Car.VINCode == string.Empty || LocalInvoice.Car.VINCode == null)
                        return;

                    var marks = unitOfWork.Marks.Find(item => item.Name == CarMarkNameBox.Text).ToList();
                    var models = unitOfWork.Models.Find(item => item.Name == CarModelNameBox.Text).ToList();
                    var cars = unitOfWork.Cars.Find(item => item.VINCode == LocalInvoice.Car.VINCode).ToList();

                    if (cars.Count == 0)
                    {
                        if (models.Count == 0)
                        {
                            if (marks.Count == 0)
                            {
                                string message = "Для " + TextBoxHelper.ActionText(Action) + " даної накладної також треба створити марку \""
                                        + CarMarkNameBox.Text + "\", модель \""
                                        + CarModelNameBox.Text + "\" та авто \""
                                        + CarVINCodeBox.Text + "\".\nВи згодні з створенням марки, моделі та авто?";
                                DialogWindow dialogWindow = new DialogWindow(message);
                                bool? dialogResult = dialogWindow.ShowDialog();
                                if (dialogResult != true)
                                    return;

                                var mark = new Mark()
                                {
                                    Name = CarMarkNameBox.Text,
                                };

                                unitOfWork.Marks.Create(mark);
                                unitOfWork.Save();

                                var model = new Model.Entities.Model()
                                {
                                    Name = CarModelNameBox.Text,
                                    Mark = mark,
                                    MarkId = mark.Id,
                                };
                                unitOfWork.Models.Create(model);
                                unitOfWork.Save();

                                LocalInvoice.Car.Model = model;
                            }
                            else if (marks.Count != 0)
                            {
                                string message = "Для " + TextBoxHelper.ActionText(Action) + " даної накладної також треба створити модель \""
                                        + CarModelNameBox.Text + "\" та авто \""
                                        + CarVINCodeBox.Text + "\".\nВи згодні з створенням моделі та авто?";
                                DialogWindow dialogWindow = new DialogWindow(message);
                                bool? dialogResult = dialogWindow.ShowDialog();
                                if (dialogResult != true)
                                    return;

                                var model = new Model.Entities.Model()
                                {
                                    Name = CarModelNameBox.Text,
                                    Mark = marks.First(),
                                    MarkId = marks.First().Id,
                                };
                                unitOfWork.Models.Create(model);
                                unitOfWork.Save();

                                LocalInvoice.Car.Model = model;
                            }
                        }
                        else if (models.Count != 0)
                        {
                            if (marks.Count == 0)
                                return;
                            else if (models.Count != 0)
                            {
                                LocalInvoice.Car.Model = models.First();
                            }
                        }
                        unitOfWork.Cars.Create(LocalInvoice.Car);
                        unitOfWork.Save();
                    }
                    else if (cars.Count != 0)
                    {
                        var car = cars.First();
                        if (car.ModelId == models.First().Id && models.First().MarkId == marks.First().Id)
                        {
                            LocalInvoice.Car = car;
                            unitOfWork.Invoices.Create(LocalInvoice);
                            unitOfWork.Save();
                            OnInvoiceCreated();
                        }
                        else
                        {
                            string message = $"При {TextBoxHelper.ActionText(Action)} даної накладної виникла проблема." +
                            $" Авто з VIN кодом {LocalInvoice.Car.VINCode} вже існує, але модель з маркою не співпадають.\n" +
                            $"Чи створювати накладну?";

                            DialogWindow dialogWindow = new DialogWindow(message);
                            bool? dialogResult = dialogWindow.ShowDialog();
                            if (dialogResult != true)
                                return;

                            LocalInvoice.Car = car;
                            unitOfWork.Invoices.Create(LocalInvoice);
                            unitOfWork.Save();
                            OnInvoiceCreated();
                        }
                    }
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
                    UpdateCreateInvoicePartButton();
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

        public void UpdateCreateInvoicePartButton()
        {
            if (IsInvoiceCreated && IsPartSelected)
            {
                CreateInvoicePartButton.IsEnabled = true;
            }
        }

        public void OnInvoiceCreated()
        {
            IsInvoiceCreated = true;
            WorkButton.Content = "Редагувати накладну";
            Action = ActionType.Edit;

            CarMarkNameBox.Text = LocalInvoice.Car.Model.Mark.Name;
            CarModelNameBox.Text = LocalInvoice.Car.Model.Name;
            CarVINCodeBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            CarInfoBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

            CarMarkNameBox.IsEnabled = false;
            CarModelNameBox.IsEnabled = false;
            CarVINCodeBox.IsEnabled = false;
            CarInfoBox.IsEnabled = false;

            UpdateCreateInvoicePartButton();
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
            InvoicePartListBox.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
        }
    }
}
