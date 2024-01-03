using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public MainWindow()
        {
            InitializeComponent();

            SetMarkHandlers();
            SetModelHandlers();
            SetCarHandlers();
            SetPartTypeHandlers();
            SetPartHandlers();
            SetInvoiceHandlers();
        }

        public void SetMarkHandlers()
        {
            ComboBoxHelper.SetDropDownOpened(MarkNameBox, unitOfWork.Marks.GetAll());

            MarkSearchButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.Marks.GetAll()
                    .Where(item => item.Name.Contains(MarkNameBox.Text))
                    .ToList();
                MarkListBox.ItemsSource = list;
            };
            MarkCreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                Mark mark = new Mark();
                mark.Name = MarkNameBox.Text;

                MarkWindow markWindow = new MarkWindow(mark, ActionType.Create);
                markWindow.Owner = this;
                markWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    MarkSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                markWindow.Show();
            };
            MarkListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var mark = MarkListBox.SelectedItem as Mark;
                if (mark != null)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.Car.Model.MarkId == mark.Id).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices);
                    invoiceSelectionWindow.Owner = this;

                    invoiceSelectionWindow.Show();
                }

                MarkListBox.UnselectAll();
            };
        }
        public void SetModelHandlers()
        {
            ComboBoxHelper.SetDropDownOpened(ModelMarkNameBox, unitOfWork.Marks.GetAll());
            ComboBoxHelper.SetDropDownOpened(ModelNameBox, unitOfWork.Models.GetAll());

            ModelSearchButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.Models.GetAll()
                    .Where(item => item.Name.Contains(ModelNameBox.Text) && item.Mark.Name.Contains(ModelMarkNameBox.Text))
                    .ToList();
                ModelListBox.ItemsSource = list;
            };
            ModelCreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                Model.Entities.Model model = new Model.Entities.Model()
                {
                    Name = ModelNameBox.Text,
                    Mark = new Mark()
                    {
                        Name = ModelMarkNameBox.Text
                    }
                };              

                ModelWindow modelWindow = new ModelWindow(model, ActionType.Create); ;
                modelWindow.Owner = this;
                modelWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    ModelSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                modelWindow.Show();
            };
            ModelListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var model = ModelListBox.SelectedItem as Model.Entities.Model;
                if (model != null)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.Car.ModelId == model.Id).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices);
                    invoiceSelectionWindow.Owner = this;

                    invoiceSelectionWindow.Show();
                }

                ModelListBox.UnselectAll();
            };
        }
        public void SetCarHandlers()
        {
            ComboBoxHelper.SetDropDownOpened(CarMarkNameBox, unitOfWork.Marks.GetAll());
            ComboBoxHelper.SetDropDownOpened(CarModelNameBox, unitOfWork.Models.GetAll());

            CarSearchButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.Cars.GetAll()
                    .Where(item => item.Info.Contains(CarInfoBox.Text) 
                        && item.Model.Name.Contains(CarModelNameBox.Text)
                        && item.VINCode.Contains(CarVINCodeBox.Text)
                        && item.Model.Mark.Name.Contains(CarMarkNameBox.Text))
                    .ToList();
                CarListBox.ItemsSource = list;
            };
            CarCreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                Car car = new Car()
                {
                    VINCode = CarVINCodeBox.Text,
                    Info = CarInfoBox.Text,
                    Model = new Model.Entities.Model()
                    {
                        Name = CarModelNameBox.Text,
                        Mark = new Mark()
                        {
                            Name = CarMarkNameBox.Text
                        }
                    }
                };


                CarWindow carWindow = new CarWindow(car, ActionType.Create); ;
                carWindow.Owner = this;
                carWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    ModelSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                carWindow.Show();
            };
            CarListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var car = CarListBox.SelectedItem as Car;
                if (car != null)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.CarId == car.Id).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices);
                    invoiceSelectionWindow.Owner = this;

                    invoiceSelectionWindow.Show();
                }

                CarListBox.UnselectAll();
            };
        }
        public void SetPartTypeHandlers()
        {
            ComboBoxHelper.SetDropDownOpened(PartTypeNameBox, unitOfWork.PartTypes.GetAll());

            PartTypeSearchButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.PartTypes.GetAll()
                    .Where(item => item.Name.Contains(PartTypeNameBox.Text))
                    .ToList();
                PartTypeListBox.ItemsSource = list;
            };
            PartTypeCreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                PartType partType = new PartType();
                partType.Name = PartTypeNameBox.Text;

                PartTypeWindow partTypeWindow = new PartTypeWindow(partType, ActionType.Create);
                partTypeWindow.Owner = this;
                partTypeWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    PartTypeSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                partTypeWindow.Show();
            };
            PartTypeListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var partType = PartTypeListBox.SelectedItem as PartType;
                if (partType != null)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.InvoiceParts
                        .Any(item2 => item2.Part.PartTypeId == partType.Id)).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices);
                    invoiceSelectionWindow.Owner = this;

                    invoiceSelectionWindow.Show();
                }

                PartTypeListBox.UnselectAll();
            };
        }
        public void SetPartHandlers()
        {
            ComboBoxHelper.SetDropDownOpened(PartPartTypeNameBox, unitOfWork.PartTypes.GetAll());
            ComboBoxHelper.SetDropDownOpened(PartNameBox, unitOfWork.Parts.GetAll());

            PartSearchButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.Parts.GetAll()
                    .Where(item => item.Name.Contains(PartNameBox.Text)
                        && item.PartType.Name.Contains(PartPartTypeNameBox.Text)
                        && item.Article.Contains(PartArticleBox.Text)
                        && item.Description.Contains(PartDescriptionBox.Text))
                    .ToList();
                PartListBox.ItemsSource = list;
            };
            PartCreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                Part part = new Part()
                {
                    Name = PartNameBox.Text,
                    Article = PartArticleBox.Text,
                    Description = PartDescriptionBox.Text,
                    PartType = new PartType()
                    {
                        Name = PartPartTypeNameBox.Text
                    }
                };

                PartWindow partWindow = new PartWindow(part, ActionType.Create);
                partWindow.Owner = this;
                partWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    PartSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                partWindow.Show();
            };
            PartListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var part = PartListBox.SelectedItem as Part;
                if (part != null)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.InvoiceParts
                        .Any(item2 => item2.PartId == part.Id)).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices);
                    invoiceSelectionWindow.Owner = this;

                    invoiceSelectionWindow.Show();
                }
            };

            PartListBox.UnselectAll();
        }
        public void SetInvoiceHandlers()
        {
            InvoiceCreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                InvoiceWindow invoiceWindow = new InvoiceWindow();
                invoiceWindow.Owner = this;

                invoiceWindow.Show();
            };
        }

        private void MarkDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var markToDelete = unitOfWork.Marks.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити марку авто \""
                             + markToDelete.Name + "\"?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Marks.Delete(markToDelete.Id);
            unitOfWork.Save();

            MarkSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void MarkEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var markToUpdate = unitOfWork.Marks.Get((int)button.Tag);

            MarkWindow markWindow = new MarkWindow(markToUpdate, ActionType.Edit);
            markWindow.Owner = this;
            markWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                MarkSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            markWindow.Show();
        }
        private void ModelDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var modelToDelete = unitOfWork.Models.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити модель авто \""
                             + modelToDelete.Mark.Name + " " + modelToDelete.Name + "\"?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Models.Delete(modelToDelete.Id);
            unitOfWork.Save();

            ModelSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void ModelEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var modelToUpdate = unitOfWork.Models.Get((int)button.Tag);

            ModelWindow modelWindow = new ModelWindow(modelToUpdate, ActionType.Edit);
            modelWindow.Owner = this;
            modelWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                ModelSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            modelWindow.Show();
        }
        private void CarDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var carToDelete = unitOfWork.Cars.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити автомобіль \""
                             + carToDelete.Model.Mark.Name + " " + carToDelete.Model.Name + " " + carToDelete.VINCode + "\"?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Cars.Delete(carToDelete.Id);
            unitOfWork.Save();

            MarkSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void CarEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var carToUpdate = unitOfWork.Cars.Get((int)button.Tag);

            CarWindow carWindow = new CarWindow(carToUpdate, ActionType.Edit);
            carWindow.Owner = this;
            carWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                MarkSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            carWindow.Show();
        }
        private void PartTypeDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partTypeToDelete = unitOfWork.PartTypes.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити тип запчастин \""
                             + partTypeToDelete.Name + "\"?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.PartTypes.Delete(partTypeToDelete.Id);
            unitOfWork.Save();

            PartTypeSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void PartTypeEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partTypeToUpdate = unitOfWork.PartTypes.Get((int)button.Tag);

            PartTypeWindow partTypeWindow = new PartTypeWindow(partTypeToUpdate, ActionType.Edit);
            partTypeWindow.Owner = this;
            partTypeWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                PartTypeSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            partTypeWindow.Show();
        }
        private void PartDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partToDelete = unitOfWork.Parts.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити запчастину \""
                             + partToDelete.PartType.Name + " " + partToDelete.Name + "\"?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Parts.Delete(partToDelete.Id);
            unitOfWork.Save();

            PartSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void PartEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partToUpdate = unitOfWork.Parts.Get((int)button.Tag);

            PartWindow partWindow = new PartWindow(partToUpdate, ActionType.Edit);
            partWindow.Owner = this;
            partWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                PartSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            partWindow.Show();
        }
    }
}
