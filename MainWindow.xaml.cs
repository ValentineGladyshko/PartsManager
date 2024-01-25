using Microsoft.SqlServer.Management.Smo;
using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using PartsManager.Model.Repositories;
using Spire.Pdf.Exporting.XPS.Schema;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
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
        public ObservableCollection<Invoice> LocalInvoices { get; set; }
        public IEnumerable<Invoice> PartnerInvoices => LocalInvoices.Where(item => !item.IsPartnerPayed && item.IsPayed);
        public IEnumerable<Invoice> ReportInvoices => LocalInvoices.Where(item => !item.IsPartnerPayed && !item.IsPayed);
        EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

        public MainWindow()
        {
            InitializeComponent();
            unitOfWork.Db.Invoices.Include(item => item.Car).Include(item => item.InvoiceParts).Include(item => item.Payments).Load();

            LocalInvoices = unitOfWork.Db.Invoices.Local;
            DataContext = this;

            SetMarkHandlers();
            SetModelHandlers();
            SetCarHandlers();
            SetPartTypeHandlers();
            SetPartHandlers();
            SetInvoiceHandlers();
            SetReportHandlers();
            SetBackupHandlers();
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
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.Car.Model.MarkId == mark.Id).OrderByDescending(item => item.Id).ToList();
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
            CarMarkNameBox.SetDropDownOpened(unitOfWork.Marks.GetAll());
            CarModelNameBox.SetDropDownOpened(unitOfWork.Models.GetAll());

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
            PartPartTypeNameBox.SetDropDownOpened(unitOfWork.PartTypes.GetAll());
            PartNameBox.SetDropDownOpened(unitOfWork.Parts.GetAll());
            PartFullNameBox.DropDownOpened += delegate
            {
                var list = unitOfWork.Parts.GetAll()
                    .Where(item => item.FullName.Contains(PartFullNameBox.Text))
                    .Select(item => item.FullName).ToList();
                list.Sort();
                PartFullNameBox.ItemsSource = list;
            };

            PartSearchButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.Parts.GetAll()
                    .Where(item => item.Name.Contains(PartNameBox.Text)
                        && item.FullName.Contains(PartFullNameBox.Text)
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
                    FullName = PartFullNameBox.Text,
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
            CreateInvoiceButton.Click += (object sender, RoutedEventArgs args) =>
            {
                InvoiceWindow invoiceWindow = new InvoiceWindow();
                invoiceWindow.Owner = this;

                invoiceWindow.Closed += (object sender1, EventArgs args1) =>
                {
                    DataGridInvoices.Items.SortDescriptions.Clear();
                    DataGridInvoices.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
                    DataGridInvoices.Items.Refresh();
                };
                invoiceWindow.Show();
            };
            DataGridInvoices.Items.SortDescriptions.Clear();
            DataGridInvoices.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
            DataGridInvoices.Items.Refresh();
            DataGridInvoices.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                var invoice = DataGridInvoices.SelectedItem as Invoice;
                if (invoice != null)
                {
                    if (DataGridInvoices.CurrentColumn.DisplayIndex == 8)
                    {
                        InvoiceInfoWindow invoiceInfoWindow = new InvoiceInfoWindow(invoice);
                        invoiceInfoWindow.Owner = this;
                        invoiceInfoWindow.Show();
                    }
                    else if (DataGridInvoices.CurrentColumn.DisplayIndex == 9)
                    {
                        PaymentWindow paymentWindow = new PaymentWindow(invoice);
                        paymentWindow.Owner = this;
                        paymentWindow.Closed += (object sender1, EventArgs args1) =>
                        {
                            DataGridInvoices.Items.SortDescriptions.Clear();
                            DataGridInvoices.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
                            DataGridInvoices.Items.Refresh();
                        };
                        paymentWindow.Show();
                    }
                    else
                    {
                        InvoiceWindow invoiceWindow = new InvoiceWindow(invoice);
                        invoiceWindow.Owner = this;

                        invoiceWindow.Closed += (object sender1, EventArgs args1) =>
                        {
                            DataGridInvoices.Items.SortDescriptions.Clear();
                            DataGridInvoices.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
                            DataGridInvoices.Items.Refresh();
                        };
                        invoiceWindow.Show();
                    }
                }
                DataGridInvoices.UnselectAll();
            };
        }
        public void SetReportHandlers()
        {
            LocalInvoices.CollectionChanged += delegate
            {
                DataGridPartnerReport.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                DataGridReport.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            };
            PartnerReportButton.Click += delegate
            {
                Workbook workbook = new Workbook();
                Worksheet worksheet = workbook.Worksheets[0];

                worksheet.Range[1, 1].Value = "Код";
                worksheet.Range[1, 2].Value = "Автомобіль";
                worksheet.Range[1, 3].Value = "Дата";
                worksheet.Range[1, 4].Value = "Сплата";
                worksheet.Range[1, 5].Value = "Партнери";
                worksheet.Range[1, 6].Value = "Закупка";
                worksheet.Range[1, 7].Value = "Продаж";
                worksheet.Range[1, 8].Value = "Партнери";
                
                List<Invoice> partnerInvoices = new List<Invoice>(PartnerInvoices);
                for (int i = 0; i < partnerInvoices.Count; i++)
                {
                    worksheet.Range[i + 2, 1].Value = partnerInvoices[i].Id.ToString();
                    worksheet.Range[i + 2, 2].Value = partnerInvoices[i].Car.FullInfo.ToString();
                    worksheet.Range[i + 2, 3].Value = partnerInvoices[i].Date.ToString("d");
                    worksheet.Range[i + 2, 4].Value = Convert.ToInt32(partnerInvoices[i].IsPayed).ToString();
                    worksheet.Range[i + 2, 5].Value = Convert.ToInt32(partnerInvoices[i].IsPartnerPayed).ToString();
                    worksheet.Range[i + 2, 6].Value = partnerInvoices[i].SumIn.ToString("C2");
                    worksheet.Range[i + 2, 7].Value = partnerInvoices[i].SumOut.ToString("C2");
                    worksheet.Range[i + 2, 8].Value = partnerInvoices[i].PartnerSum.ToString("C2");
                }
                var formula = $"=SUM(Sheet1!$H$2:H${partnerInvoices.Count + 1})";
                worksheet.Range[partnerInvoices.Count + 2, 8].Formula = formula;
                worksheet.Range[1, 1, 1, 8].Style.Color = System.Drawing.Color.LightGray;
                worksheet.AllocatedRange.AutoFitColumns();

                var directory = AppDomain.CurrentDomain.BaseDirectory + "reports";
                Directory.CreateDirectory(directory);
                workbook.SaveToFile($"reports/partner report{DateTime.Now: yyyy-MM-dd HH-mm-ss}.xlsx", ExcelVersion.Version2016);
            };
            ReportButton.Click += delegate
            {
                Workbook workbook = new Workbook();
                Worksheet worksheet = workbook.Worksheets[0];

                worksheet.Range[1, 1].Value = "Код";
                worksheet.Range[1, 2].Value = "Автомобіль";
                worksheet.Range[1, 3].Value = "Дата";
                worksheet.Range[1, 4].Value = "Сплата";
                worksheet.Range[1, 5].Value = "Партнери";
                worksheet.Range[1, 6].Value = "Закупка";
                worksheet.Range[1, 7].Value = "Продаж";

                List<Invoice> reportInvoices = new List<Invoice>(ReportInvoices);
                for (int i = 0; i < reportInvoices.Count; i++)
                {
                    worksheet.Range[i + 2, 1].Value = reportInvoices[i].Id.ToString();
                    worksheet.Range[i + 2, 2].Value = reportInvoices[i].Car.FullInfo.ToString();
                    worksheet.Range[i + 2, 3].Value = reportInvoices[i].Date.ToString("d");
                    worksheet.Range[i + 2, 4].Value = Convert.ToInt32(reportInvoices[i].IsPayed).ToString();
                    worksheet.Range[i + 2, 5].Value = Convert.ToInt32(reportInvoices[i].IsPartnerPayed).ToString();
                    worksheet.Range[i + 2, 6].Value = reportInvoices[i].SumIn.ToString("C2");
                    worksheet.Range[i + 2, 7].Value = reportInvoices[i].SumOut.ToString("C2");
                }
                var formula = $"=SUM(Sheet1!$G$2:G${reportInvoices.Count + 1})";
                worksheet.Range[reportInvoices.Count + 2, 7].Formula = formula;
                worksheet.Range[1, 1, 1, 8].Style.Color = System.Drawing.Color.LightGray;
                worksheet.AllocatedRange.AutoFitColumns();

                var directory = AppDomain.CurrentDomain.BaseDirectory + "reports";
                Directory.CreateDirectory(directory);
                workbook.SaveToFile($"reports/report{DateTime.Now: yyyy-MM-dd HH-mm-ss}.xlsx", ExcelVersion.Version2016);
            };
        }
        public void SetBackupHandlers()
        {
            CreateBackupButton.Click += delegate
            {
                var date = DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss");
                JsonBackupHelper.Backup($"jsonBackup{date}");
                var backup = BackupHelper.CreateBackup(unitOfWork.Db.Database.Connection.Database, $"dbBackup{date}");

                var server = new Server(unitOfWork.Db.Database.Connection.DataSource);

                backup.Complete += delegate
                {
                    StatusTextBlock.Text = "Резервну копію створено!";
                };

                backup.SqlBackup(server);
            };
            ChooseBackupButton.Click +=  delegate
            {
                var path = BackupHelper.ChooseRestore();
                Restore(path);
            };
            LoadBackupButton.Click += delegate
            {
                var directory = AppDomain.CurrentDomain.BaseDirectory + "backups";
                Directory.CreateDirectory(directory);
                var files = Directory.GetFiles(directory);
                var fileInfos = new List<FileInfo>();
                foreach (var file in files)
                {
                    fileInfos.Add(new FileInfo(file));
                }
                fileInfos = fileInfos.Where(item => item.Extension == ".bak").OrderByDescending(item => item.Name).ToList();
                BackupListBox.ItemsSource = fileInfos;
            };
            ChooseJsonBackupButton.Click += delegate
            {
                var path = JsonBackupHelper.ChooseRestore();
                if (path != null)
                {
                    if (BackupHelper.AskRestore(System.IO.Path.GetFileName(path)) == true)
                    {
                        var dbName = unitOfWork.Db.Database.Connection.Database;
                        var server = new Server(unitOfWork.Db.Database.Connection.DataSource);
                        string backupName = "JsonRestoreBackup";

                        var backup = BackupHelper.CreateBackup(dbName, backupName);
                        backup.SqlBackup(server);

                        JsonBackupHelper.Restore(path);
                        unitOfWork.Reload();
                        unitOfWork.Db.Invoices.Include(item => item.Car).Include(item => item.InvoiceParts).Include(item => item.Payments).Load();
                        LocalInvoices = unitOfWork.Db.Invoices.Local;
                        DataGridInvoices.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                        DataGridPartnerReport.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                        DataGridReport.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                    };
                }
                else
                    StatusTextBlock.Text = "";
            };
            LoadBackupButton2.Click += delegate
            {
                var directory = AppDomain.CurrentDomain.BaseDirectory + "backups";
                var files = Directory.GetFiles(directory);
                var fileInfos = new List<FileInfo>();
                foreach (var file in files)
                {
                    fileInfos.Add(new FileInfo(file));
                }
                fileInfos = fileInfos.Where(item => item.Extension == ".bak").OrderByDescending(item => item.Name).ToList();
                BackupListBox.ItemsSource = fileInfos;
            };
            BackupListBox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                var fileInfo = BackupListBox.SelectedItem as FileInfo;
                if (fileInfo != null)
                {
                    Restore(fileInfo.FullName);
                }
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
        private void ViewInvoiceOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var invoice = unitOfWork.Invoices.Get((int)button.Tag);

            InvoiceInfoWindow invoiceInfoWindow = new InvoiceInfoWindow(invoice);
            invoiceInfoWindow.Owner = this;
            invoiceInfoWindow.Show();
        }
        private void PaymentInvoiceOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var invoice = unitOfWork.Invoices.Get((int)button.Tag);

            PaymentWindow paymentWindow = new PaymentWindow(invoice);
            paymentWindow.Owner = this;
            paymentWindow.Show();
        }
        private void Restore(string path)
        {
            if (path != null)
            {
                if (BackupHelper.AskRestore(System.IO.Path.GetFileName(path)) == true)
                {
                    var dbName = unitOfWork.Db.Database.Connection.Database;
                    var server = new Server(unitOfWork.Db.Database.Connection.DataSource);
                    string backupName = "dbRestoreBackup";

                    if (!BackupHelper.CheckSameName(backupName, path))
                    {
                        var backup = BackupHelper.CreateBackup(dbName, backupName);

                        backup.SqlBackup(server);
                    }
                    var restore = BackupHelper.CreateRestore(dbName, path);
                    restore.Complete += delegate
                    {
                        StatusTextBlock.Text = "Резервну копію завантажено!";
                    };
                    server.KillAllProcesses(dbName);
                    restore.SqlRestore(server);
                    unitOfWork.Reload();
                    unitOfWork.Db.Invoices.Include(item => item.Car).Include(item => item.InvoiceParts).Include(item => item.Payments).Load();
                    LocalInvoices = unitOfWork.Db.Invoices.Local;
                    DataGridInvoices.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                    DataGridPartnerReport.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                    DataGridReport.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                }
            }
            else
                StatusTextBlock.Text = "";
        }
    }
}
