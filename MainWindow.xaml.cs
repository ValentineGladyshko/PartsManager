using Microsoft.SqlServer.Management.Smo;
using PartsManager.BaseHandlers;
using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using PdfSharp.Xps;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Text.Json;
using PartsManager.Model.Interfaces;

namespace PartsManager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public IEnumerable<PartnerPayment> PartnerPayments => unitOfWork.PartnerPayments.GetAll().ToList();
        public IEnumerable<PartnerPayment> ReportPartnerPayments => PartnerPayments.Where(item => !item.IsActive && item.PaymentAmountIn > 0);
        public IEnumerable<Invoice> LocalInvoices => DatabaseInvoices.Where(item => !item.IsMine);
        public IEnumerable<Invoice> MyInvoices => DatabaseInvoices.Where(item => item.IsMine);
        public ObservableCollection<Invoice> DatabaseInvoices { get; set; }
        public IEnumerable<Invoice> PayedInvoices => DatabaseInvoices.Where(item => !item.IsPartnerPayed && item.IsPayed && !item.IsBill);
        public IEnumerable<Invoice> UnpayedInvoices => DatabaseInvoices.Where(item => !item.IsPartnerPayed && !item.IsPayed && !item.IsBill);
        public IEnumerable<Invoice> PartnerInvoices => LocalInvoices.Where(item => !item.IsPartnerPayed && !item.IsBill);
        public IEnumerable<Invoice> ReportInvoices => DatabaseInvoices.Where(item => !item.IsBill);
        public decimal UnpayedInvoicesResidueSum => UnpayedInvoices.Sum(item => item.Residue);
        public decimal UnpayedInvoicesSumInDelivery => UnpayedInvoices.Sum(item => item.SumInDelivery);
        public decimal InvoicePaymentSum => PartnerPayments.Where(item => !item.IsActive).Sum(item => item.InvoicePayment);
        public decimal PartnerInvoicesPartnerSum => PayedInvoices.Sum(item => item.PartnerSum) - InvoicePaymentSum;
        public decimal PaymentAmountOutSum => PartnerPayments.Where(item => !item.IsActive && item.PaymentAmountIn > 0).Sum(item => item.PaymentAmountOut);
        public int InvoiceLastCorrectId { get; set; }

        private readonly EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public MainWindow()
        {
            InitializeComponent();
            unitOfWork.Db.Invoices.Include(item => item.Car).Include(item => item.InvoiceParts).Include(item => item.Payments).Load();
            DatabaseInvoices = unitOfWork.Db.Invoices.Local;
            DatabaseInvoices.CollectionChanged += delegate
            {
                Refresh();
            };

            DataContext = this;
            SetMarkHandlers();
            SetModelHandlers();
            SetCarHandlers();
            SetPartTypeHandlers();
            SetPartHandlers();
            SetInvoiceHandlers();
            SetReportHandlers();
            SetBackupHandlers();
            SetOilPartHandlers();
        }

        public void SetMarkHandlers()
        {
            ComboBoxHelper.SetDropDownOpened(MarkNameBox, unitOfWork.Marks.GetAll());

            MarkSearchButton.Click += delegate
            {
                var list = unitOfWork.Marks.GetAll()
                    .Where(item => item.Name.Contains(MarkNameBox.Text))
                    .ToList();
                MarkListBox.ItemsSource = list;
            };
            MarkCreateButton.Click += delegate
            {
                var mark = new Mark
                {
                    Name = MarkNameBox.Text
                };

                var markWindow = new MarkWindow(mark, ActionType.Create)
                {
                    Owner = this
                };
                markWindow.Closed += delegate
                {
                    MarkSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                };

                markWindow.Show();
            };
            MarkListBox.SelectionChanged += delegate
            {
                if (MarkListBox.SelectedItem is Mark mark)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.Car.Model.MarkId == mark.Id).OrderByDescending(item => item.Id).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices)
                    {
                        Owner = this
                    };

                    invoiceSelectionWindow.Show();
                }

                MarkListBox.UnselectAll();
            };
        }
        public void SetModelHandlers()
        {
            ModelMarkNameBox.SetDropDownOpened(unitOfWork.Marks.GetAll());
            ModelNameBox.SetDropDownOpened(unitOfWork.Models.GetAll());

            ModelSearchButton.Click += delegate
            {
                var list = unitOfWork.Models.GetAll()
                    .Where(item => item.Name.Contains(ModelNameBox.Text) && item.Mark.Name.Contains(ModelMarkNameBox.Text))
                    .ToList();
                ModelListBox.ItemsSource = list;
            };
            ModelCreateButton.Click += delegate
            {
                Model.Entities.Model model = new Model.Entities.Model()
                {
                    Name = ModelNameBox.Text,
                    Mark = new Mark()
                    {
                        Name = ModelMarkNameBox.Text
                    }
                };

                var modelWindow = new ModelWindow(model, ActionType.Create)
                {
                    Owner = this
                };
                modelWindow.Closed += delegate
                {
                    ModelSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                };

                modelWindow.Show();
            };
            ModelListBox.SelectionChanged += delegate
            {
                if (ModelListBox.SelectedItem is Model.Entities.Model model)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.Car.ModelId == model.Id).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices)
                    {
                        Owner = this
                    };

                    invoiceSelectionWindow.Show();
                }

                ModelListBox.UnselectAll();
            };
        }
        public void SetCarHandlers()
        {
            CarMarkNameBox.SetDropDownOpened(unitOfWork.Marks.GetAll());
            CarModelNameBox.SetDropDownOpened(unitOfWork.Models.GetAll());

            CarSearchButton.Click += delegate
            {
                var list = unitOfWork.Cars.GetAll()
                    .Where(item => item.Info.Contains(CarInfoBox.Text) 
                        && item.Model.Name.Contains(CarModelNameBox.Text)
                        && item.VINCode.Contains(CarVINCodeBox.Text)
                        && item.Model.Mark.Name.Contains(CarMarkNameBox.Text))
                    .ToList();
                CarListBox.ItemsSource = list;
            };
            CarCreateButton.Click += delegate
            {
                var car = new Car()
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


                var carWindow = new CarWindow(car, ActionType.Create)
                {
                    Owner = this
                };
                carWindow.Closed += delegate
                {
                    ModelSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                };

                carWindow.Show();
            };
            CarListBox.SelectionChanged += delegate
            {
                if (CarListBox.SelectedItem is Car car)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.CarId == car.Id).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices)
                    {
                        Owner = this
                    };

                    invoiceSelectionWindow.Show();
                }

                CarListBox.UnselectAll();
            };
        }
        public void SetPartTypeHandlers()
        {
            PartTypeNameBox.SetDropDownOpened(unitOfWork.PartTypes.GetAll());

            PartTypeSearchButton.Click += delegate
            {
                var list = unitOfWork.PartTypes.GetAll()
                    .Where(item => item.Name.Contains(PartTypeNameBox.Text))
                    .ToList();
                PartTypeListBox.ItemsSource = list;
            };
            PartTypeCreateButton.Click += delegate
            {
                var partType = new PartType
                {
                    Name = PartTypeNameBox.Text
                };

                var partTypeWindow = new PartTypeWindow(partType, ActionType.Create)
                {
                    Owner = this
                };
                partTypeWindow.Closed += delegate
                {
                    PartTypeSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                };

                partTypeWindow.Show();
            };
            PartTypeListBox.SelectionChanged += delegate
            {
                if (PartTypeListBox.SelectedItem is PartType partType)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.InvoiceParts
                        .Any(item2 => item2.Part.PartTypeId == partType.Id)).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices)
                    {
                        Owner = this
                    };

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

            PartSearchButton.Click += delegate
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
            PartCreateButton.Click += delegate
            {
                var part = new Part()
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

                var partWindow = new PartWindow(part, ActionType.Create)
                {
                    Owner = this
                };
                partWindow.Closed += delegate
                {
                    PartSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                };

                partWindow.Show();
            };
            PartListBox.SelectionChanged += delegate
            {
                if (PartListBox.SelectedItem is Part part)
                {
                    var invoices = unitOfWork.Invoices.GetAll().Where(item => item.InvoiceParts
                        .Any(item2 => item2.PartId == part.Id)).ToList();
                    var invoiceSelectionWindow = new InvoiceSelectionWindow(invoices)
                    {
                        Owner = this
                    };

                    invoiceSelectionWindow.Show();
                }
                PartListBox.UnselectAll();
            };
        }
        public void SetInvoiceHandlers()
        {
            Refresh();

            CreatePartnerPaymentButton.Click += delegate
            {
                var payPartnerWindow = new PayPartnerWindow()
                {
                    Owner = this
                };

                payPartnerWindow.Closed += delegate
                {
                    Refresh();
                };
                payPartnerWindow.Show();
            };
            CreateInvoiceButton.Click += delegate
            {
                var invoiceWindow = new InvoiceWindow(false)
                {
                    Owner = this
                };

                invoiceWindow.Closed += delegate
                {
                    Refresh();
                };
                invoiceWindow.Show();
            };
            CreateMyInvoiceButton.Click += delegate
            {
                var invoiceWindow = new MyInvoiceWindow(true)
                {
                    Owner = this
                };

                invoiceWindow.Closed += delegate
                {
                    Refresh();
                };
                invoiceWindow.Show();
            };

            DataGridPartnerPayments.SelectionChanged += delegate
            {
                if (DataGridPartnerPayments.SelectedItem is PartnerPayment partnerPayment)
                {
                    if (DataGridPartnerPayments.CurrentColumn.DisplayIndex == 1)
                    {
                        var payPartnerWindow = new PayPartnerWindow(partnerPayment)
                        {
                            Owner = this
                        };

                        payPartnerWindow.Closed += delegate
                        {
                            Refresh();
                        };
                        payPartnerWindow.Show();
                    }
                }
                DataGridPartnerPayments.UnselectAll();
            };
            DataGridInvoices.SelectionChanged += delegate
            {
                if (DataGridInvoices.SelectedItem is Invoice invoice)
                {
                    if (DataGridInvoices.CurrentColumn.DisplayIndex == 8)
                    {
                        var invoiceInfoWindow = new InvoiceInfoWindow(invoice)
                        {
                            Owner = this
                        };
                        invoiceInfoWindow.Show();
                    }
                    else if (DataGridInvoices.CurrentColumn.DisplayIndex == 9)
                    {
                        var paymentWindow = new PaymentWindow(invoice)
                        {
                            Owner = this
                        };
                        paymentWindow.Closed += delegate
                        {
                            Refresh();
                        };
                        paymentWindow.Show();
                    }
                    else if (DataGridInvoices.CurrentColumn.DisplayIndex == 10)
                    {
                    }
                    else if (DataGridInvoices.CurrentColumn.DisplayIndex == 11)
                    {
                    }
                    else
                    {
                        var invoiceWindow = new InvoiceWindow(invoice)
                        {
                            Owner = this
                        };

                        invoiceWindow.Closed += delegate
                        {
                            Refresh();
                        };
                        invoiceWindow.Show();
                    }
                }
                DataGridInvoices.UnselectAll();
            };
            DataGridMyInvoices.SelectionChanged += delegate
            {
                if (DataGridMyInvoices.SelectedItem is Invoice invoice)
                {
                    if (DataGridMyInvoices.CurrentColumn.DisplayIndex == 9)
                    {
                        var invoiceInfoWindow = new MyInvoiceInfoWindow(invoice)
                        {
                            Owner = this
                        };
                        invoiceInfoWindow.Show();
                    }
                    else if (DataGridMyInvoices.CurrentColumn.DisplayIndex == 10)
                    {
                        var paymentWindow = new PaymentWindow(invoice)
                        {
                            Owner = this
                        };
                        paymentWindow.Closed += delegate
                        {
                            Refresh();
                        };
                        paymentWindow.Show();
                    }
                    else if (DataGridMyInvoices.CurrentColumn.DisplayIndex == 11)
                    {
                    }
                    else if (DataGridMyInvoices.CurrentColumn.DisplayIndex == 12)
                    {
                    }
                    else
                    {
                        var invoiceWindow = new MyInvoiceWindow(invoice)
                        {
                            Owner = this
                        };

                        invoiceWindow.Closed += delegate
                        {
                            Refresh();
                        };
                        invoiceWindow.Show();
                    }
                }
                DataGridMyInvoices.UnselectAll();
            };
        }
        public void SetReportHandlers()
        {
            PartnerReportButton.Click += delegate
            {
                var workbook = new Workbook();
                var worksheet = workbook.Worksheets[0];

                worksheet.Range[1, 1].Value = "Код";
                worksheet.Range[1, 2].Value = "Автомобіль";
                worksheet.Range[1, 3].Value = "Дата";
                worksheet.Range[1, 4].Value = "Сплата";
                worksheet.Range[1, 5].Value = "Партнери";
                worksheet.Range[1, 6].Value = "Закупка";
                worksheet.Range[1, 7].Value = "Продаж";
                worksheet.Range[1, 8].Value = "Партнери";

                var partnerInvoices = new List<Invoice>(PayedInvoices);
                var s = 0.0m;
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
                    s += partnerInvoices[i].PartnerSum;
                }
                var formula = $"=SUM(Sheet1!$H$2:H${partnerInvoices.Count + 1})";
                var t = partnerInvoices.Sum(item => item.PartnerSum);
                worksheet.Range[partnerInvoices.Count + 2, 8].Formula = formula;
                worksheet.Range[1, 1, 1, 8].Style.Color = System.Drawing.Color.LightGray;
                worksheet.AllocatedRange.AutoFitColumns();

                var directory = AppDomain.CurrentDomain.BaseDirectory + "reports";
                Directory.CreateDirectory(directory);
                workbook.SaveToFile($"reports/partner report{DateTime.Now: yyyy-MM-dd HH-mm-ss}.xlsx", ExcelVersion.Version2016);
            };
            UnpayedReportButton.Click += delegate
            {
                var workbook = new Workbook();
                var worksheet = workbook.Worksheets[0];

                worksheet.Range[1, 1].Value = "Код";
                worksheet.Range[1, 2].Value = "Автомобіль";
                worksheet.Range[1, 3].Value = "Дата";
                worksheet.Range[1, 4].Value = "Сплата";
                worksheet.Range[1, 5].Value = "Партнери";
                worksheet.Range[1, 6].Value = "Закупка";
                worksheet.Range[1, 7].Value = "Продаж";
                worksheet.Range[1, 8].Value = "Партнери";

                var unpayedInvoices = new List<Invoice>(UnpayedInvoices);
                for (int i = 0; i < unpayedInvoices.Count; i++)
                {
                    worksheet.Range[i + 2, 1].Value = unpayedInvoices[i].Id.ToString();
                    worksheet.Range[i + 2, 2].Value = unpayedInvoices[i].Car.FullInfo.ToString();
                    worksheet.Range[i + 2, 3].Value = unpayedInvoices[i].Date.ToString("d");
                    worksheet.Range[i + 2, 4].Value = Convert.ToInt32(unpayedInvoices[i].IsPayed).ToString();
                    worksheet.Range[i + 2, 5].Value = Convert.ToInt32(unpayedInvoices[i].IsPartnerPayed).ToString();
                    worksheet.Range[i + 2, 6].Value = unpayedInvoices[i].SumIn.ToString("C2");
                    worksheet.Range[i + 2, 7].Value = unpayedInvoices[i].Residue.ToString("C2");
                }
                var formula = $"=SUM(Sheet1!$G$2:G${unpayedInvoices.Count + 1})";
                worksheet.Range[unpayedInvoices.Count + 2, 7].Formula = formula;
                worksheet.Range[1, 1, 1, 8].Style.Color = System.Drawing.Color.LightGray;
                worksheet.AllocatedRange.AutoFitColumns();

                var directory = AppDomain.CurrentDomain.BaseDirectory + "reports";
                Directory.CreateDirectory(directory);
                workbook.SaveToFile($"reports/unpayed report{DateTime.Now: yyyy-MM-dd HH-mm-ss}.xlsx", ExcelVersion.Version2016);
            };
            ReportButton.Click += delegate
            {
                var workbook = new Workbook();
                var worksheet = workbook.Worksheets[0];

                worksheet.Range[1, 1].Value = "Код";
                worksheet.Range[1, 2].Value = "Автомобіль";
                worksheet.Range[1, 3].Value = "Дата";
                worksheet.Range[1, 4].Value = "Сплата";
                worksheet.Range[1, 5].Value = "Партнери";
                worksheet.Range[1, 6].Value = "Закупка";
                worksheet.Range[1, 7].Value = "Продаж";
                worksheet.Range[1, 8].Value = "Доставка";
                worksheet.Range[1, 9].Value = "Сума";
                worksheet.Range[1, 10].Value = "Сума Азов";
                worksheet.Range[1, 11].Value = "Відсоток партнерам %";
                worksheet.Range[1, 12].Value = "Податок %";             
                worksheet.Range[1, 13].Value = "Азов?";

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
                    worksheet.Range[i + 2, 8].Value = reportInvoices[i].DeliveryPrice.ToString("C2");                   
                    worksheet.Range[i + 2, 9].Value = reportInvoices[i].SumTotal.ToString("C2");
                    worksheet.Range[i + 2, 10].Value = reportInvoices[i].SumTotal2.ToString("C2");
                    worksheet.Range[i + 2, 11].Value = reportInvoices[i].PartnerInterest.ToString("C2");
                    worksheet.Range[i + 2, 12].Value = reportInvoices[i].TaxInterest.ToString("C2");
                    worksheet.Range[i + 2, 13].Value = Convert.ToInt32(reportInvoices[i].IsMine).ToString();
                }
                var formula = $"=SUM(Sheet1!$G$2:G${reportInvoices.Count + 1})";
                var formula2 = $"=SUM(Sheet1!F2:F{reportInvoices.Count + 1})";
                var formula3 = $"=SUM(Sheet1!H2:H{reportInvoices.Count + 1})";
                var formula4 = $"=SUM(Sheet1!I2:I{reportInvoices.Count + 1})";
                var formula5 = $"=SUM(Sheet1!J2:J{reportInvoices.Count + 1})";
                worksheet.Range[reportInvoices.Count + 2, 7].Formula = formula;
                worksheet.Range[reportInvoices.Count + 2, 6].Formula = formula2;
                worksheet.Range[reportInvoices.Count + 2, 8].Formula = formula3;
                worksheet.Range[reportInvoices.Count + 2, 9].Formula = formula4;
                worksheet.Range[reportInvoices.Count + 2, 10].Formula = formula5;
                worksheet.Range[1, 1, 1, 13].Style.Color = System.Drawing.Color.LightGray;
                worksheet.AllocatedRange.AutoFitColumns();

                var directory = AppDomain.CurrentDomain.BaseDirectory + "reports";
                Directory.CreateDirectory(directory);
                workbook.SaveToFile($"reports/report{DateTime.Now: yyyy-MM-dd HH-mm-ss}.xlsx", ExcelVersion.Version2016);
            };
            PaymentReportButton.Click += delegate
            {
                var workbook = new Workbook();
                var worksheet = workbook.Worksheets[0];

                worksheet.Range[1, 1].Value = "Опис";
                worksheet.Range[1, 2].Value = "Сума";
                worksheet.Range[1, 3].Value = "Дата";

                var reportPartnerPayments = new List<PartnerPayment>(ReportPartnerPayments);
                for (int i = 0; i < reportPartnerPayments.Count; i++)
                {
                    worksheet.Range[i + 2, 1].Value = reportPartnerPayments[i].Info.ToString();
                    worksheet.Range[i + 2, 2].Value = reportPartnerPayments[i].PaymentAmountOut.ToString("C2");
                    worksheet.Range[i + 2, 3].Value = reportPartnerPayments[i].DateOut.ToString("d");
                }
                var formula = $"=SUM(Sheet1!$H$2:H${reportPartnerPayments.Count + 1})";
                worksheet.Range[reportPartnerPayments.Count + 2, 8].Formula = formula;
                worksheet.Range[1, 1, 1, 8].Style.Color = System.Drawing.Color.LightGray;
                worksheet.AllocatedRange.AutoFitColumns();

                var directory = AppDomain.CurrentDomain.BaseDirectory + "reports";
                Directory.CreateDirectory(directory);
                workbook.SaveToFile($"reports/payment report{DateTime.Now: yyyy-MM-dd HH-mm-ss}.xlsx", ExcelVersion.Version2016);
            };
        }
        public void SetBackupHandlers()
        {
            Closing += (object sender, CancelEventArgs e) =>
            {
                string message = "Ви впевнені що хочете закрити програму?";
                var dialogWindow = new DialogWindow(message);
                bool? dialogResult = dialogWindow.ShowDialog();
                if (dialogResult != true)
                    e.Cancel = true;
                else
                    e.Cancel = false;
            };
            CreateBackupButton.Click += delegate
            {
                var date = DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss");
                JsonBackupHelper.Backup($"jsonBackup{date}");
                var backup = BackupHelper.CreateBackup(unitOfWork.Db.Database.Connection.Database, $"dbBackup{date}");

                var server = new Server(unitOfWork.Db.Database.Connection.DataSource);

                backup.Complete += delegate
                {
                    StatusTextBlock.Text = "Резервну копію створено!";
                    StatusTextBlock.Foreground = Brushes.DarkGreen;
                };

                backup.SqlBackup(server);
            };
            ChooseBackupButton.Click += delegate
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
                    if (BackupHelper.AskRestore(Path.GetFileName(path)) == true)
                    {
                        var dbName = unitOfWork.Db.Database.Connection.Database;
                        var server = new Server(unitOfWork.Db.Database.Connection.DataSource);
                        string backupName = "JsonRestoreBackup";

                        var backup = BackupHelper.CreateBackup(dbName, backupName);
                        backup.SqlBackup(server);

                        if (JsonBackupHelper.Restore(path))
                        {
                            StatusTextBlock.Text = "Резервну копію завантажено!";
                            StatusTextBlock.Foreground = Brushes.DarkGreen;
                        }
                        else
                        {
                            StatusTextBlock.Text = "Помилка завантаження резервної копії!";
                            StatusTextBlock.Foreground = Brushes.DarkRed;
                        }
                        unitOfWork.Reload();
                        unitOfWork.Db.Invoices.Include(item => item.Car).Include(item => item.InvoiceParts).Include(item => item.Payments).Load();
                        DatabaseInvoices = unitOfWork.Db.Invoices.Local;
                        DatabaseInvoices.CollectionChanged += delegate
                        {
                            Refresh();
                        };
                        Refresh();
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
            BackupListBox.SelectionChanged += delegate
            {
                if (BackupListBox.SelectedItem is FileInfo fileInfo)
                {
                    Restore(fileInfo.FullName);
                }
            };
            RearrangeIDButton.Click += delegate
            {
                var date = DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss");
                var path = JsonBackupHelper.ChooseRestore();
                if (path != null)
                {
                    var jsonString = File.ReadAllText(path);
                    var databaseBackup = JsonSerializer.Deserialize<DatabaseBackup>(jsonString);
                    var jsonDatabase = new JsonDatabase(databaseBackup);
                    var ty = jsonDatabase.Parts;
                    jsonDatabase.RearrangeID(InvoiceLastCorrectId);
                    ty = jsonDatabase.Parts;
                    var tu = databaseBackup.Parts;
                    var backupName = $"jsonBackup{date}";
                    var directory = AppDomain.CurrentDomain.BaseDirectory + "jsonBackups";

                    Directory.CreateDirectory(directory);
                    var path1 = Path.Combine(directory, backupName);
                    path1 = Path.ChangeExtension(path1, "json");


                    string jsonString1 = JsonSerializer.Serialize(databaseBackup);

                    File.WriteAllText(path1, jsonString1);
                }
            };
            FixPartnerPayedButton.Click += delegate
            {
                //foreach (var invoice in ReportInvoices)
                //{
                //    if (invoice != null)
                //    {
                //        if (invoice.IsPartnerPayed)
                //        {
                //            invoice.PartnerPayed = invoice.PartnerSum;
                //            unitOfWork.Invoices.Update(invoice);
                //        }
                //    }
                //};
                //unitOfWork.Save();
                //Refresh();
            };
            //PayPartnersButton.Click += delegate
            //{
            //    var payPartnerWindow = new PayPartnerWindow()
            //    {
            //        Owner = this
            //    };

            //    payPartnerWindow.Closed += delegate
            //    {
            //        unitOfWork.Reload();
            //        unitOfWork.Db.Invoices.Include(item => item.Car).Include(item => item.InvoiceParts).Include(item => item.Payments).Load();
            //        DatabaseInvoices = unitOfWork.Db.Invoices.Local;
            //        DatabaseInvoices.CollectionChanged += delegate
            //        {
            //            Refresh();
            //        };
            //        Refresh();
            //    };
            //    payPartnerWindow.Show();
            //};
        }
        public void SetOilPartHandlers()
        {
            ManufacturerBox.SetDropDownOpened(unitOfWork.Manufacturers.GetAll());
            SaeQualityStandardBox.SetDropDownOpened(unitOfWork.SaeQualityStandards.GetAll());
            ManufacturerStandardBox.SetDropDownOpened(unitOfWork.ManufacturerStandards.GetAll());
            ApiStandardBox.SetDropDownOpened(unitOfWork.ApiStandards.GetAll());
            ManufacturerBox.SetTextChangedFirstCharToUpper();
            SaeQualityStandardBox.SetTextChangedFirstCharToUpper();
            ManufacturerStandardBox.SetTextChangedFirstCharToUpper();
            ApiStandardBox.SetTextChangedFirstCharToUpper();
            OilPartCreateButton.Click += delegate
            {
                var oilPartWindow = new OilPartWindow
                {
                    Owner = this
                };

                oilPartWindow.Closed += delegate
                {
                    OilPartSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                };
                oilPartWindow.Show();
            };
            OilPartSearchButton.Click += delegate
            {
                var infos = unitOfWork.AdditionalInfos.GetAll()
                    .Where(item => item.Manufacturer.Name.Contains(ManufacturerBox.Text)
                        && item.SaeQualityStandard.Name.Contains(SaeQualityStandardBox.Text))
                    .ToList();

                infos = infos
                    .Where(item => item.Part.PartApiStandards.Any(item2 => item2.ApiStandard.Name.Contains(ApiStandardBox.Text)))
                    .Where(item => item.Part.PartManufacturerStandards.Any(item2 => item2.ManufacturerStandard.Name.Contains(ManufacturerStandardBox.Text)))
                    .ToList();

                OilPartListBox.ItemsSource = infos;
            };
            OilPartListBox.SelectionChanged += delegate
            {
                if (OilPartListBox.SelectedItem is AdditionalInfo info)
                {
                    var oilPartWindow = new OilPartWindow(info.Part)
                    {
                        Owner = this
                    };
                    oilPartWindow.Closed += delegate
                    {
                        OilPartSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    };
                    oilPartWindow.Show();
                }

                OilPartListBox.UnselectAll();
            };
        }

        private void MarkDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var markToDelete = unitOfWork.Marks.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити марку авто \""
                             + markToDelete.Name + "\"?";

            var dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Marks.Delete(markToDelete.Id);
            unitOfWork.Save();

            MarkSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        private void MarkEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var markToUpdate = unitOfWork.Marks.Get((int)button.Tag);

            var markWindow = new MarkWindow(markToUpdate, ActionType.Edit)
            {
                Owner = this
            };
            markWindow.Closed += delegate
            {
                MarkSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            };
            markWindow.Show();
        }
        private void ModelDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var modelToDelete = unitOfWork.Models.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити модель авто \""
                             + modelToDelete.Mark.Name + " " + modelToDelete.Name + "\"?";

            var dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Models.Delete(modelToDelete.Id);
            unitOfWork.Save();

            ModelSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        private void ModelEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var modelToUpdate = unitOfWork.Models.Get((int)button.Tag);

            var modelWindow = new ModelWindow(modelToUpdate, ActionType.Edit)
            {
                Owner = this
            };
            modelWindow.Closed += delegate
            {
                ModelSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            };
            modelWindow.Show();
        }
        private void CarDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var carToDelete = unitOfWork.Cars.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити автомобіль \""
                             + carToDelete.Model.Mark.Name + " " + carToDelete.Model.Name + " " + carToDelete.VINCode + "\"?";

            var dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Cars.Delete(carToDelete.Id);
            unitOfWork.Save();

            MarkSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        private void CarEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var carToUpdate = unitOfWork.Cars.Get((int)button.Tag);

            var carWindow = new CarWindow(carToUpdate, ActionType.Edit)
            {
                Owner = this
            };
            carWindow.Closed += delegate
            {
                MarkSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            };
            carWindow.Show();
        }
        private void PartTypeDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partTypeToDelete = unitOfWork.PartTypes.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити тип запчастин \""
                             + partTypeToDelete.Name + "\"?";

            var dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.PartTypes.Delete(partTypeToDelete.Id);
            unitOfWork.Save();

            PartTypeSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        private void PartTypeEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partTypeToUpdate = unitOfWork.PartTypes.Get((int)button.Tag);

            var partTypeWindow = new PartTypeWindow(partTypeToUpdate, ActionType.Edit)
            {
                Owner = this
            };
            partTypeWindow.Closed += delegate
            {
                PartTypeSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            };
            partTypeWindow.Show();
        }
        private void PartDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partToDelete = unitOfWork.Parts.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити запчастину \""
                             + partToDelete.Name + "\"?";

            var dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Parts.Delete(partToDelete.Id);
            unitOfWork.Save();

            PartSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        private void PartEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var partToUpdate = unitOfWork.Parts.Get((int)button.Tag);

            var partWindow = new PartWindow(partToUpdate, ActionType.Edit)
            {
                Owner = this
            };
            partWindow.Closed += delegate
            {
                PartSearchButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            };
            partWindow.Show();
        }
        private void ViewInvoiceOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var invoice = unitOfWork.Invoices.Get((int)button.Tag);

            var invoiceInfoWindow = new InvoiceInfoWindow(invoice)
            {
                Owner = this
            };
            invoiceInfoWindow.Show();
        }
        private void ViewPackingListOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var invoice = unitOfWork.Invoices.Get((int)button.Tag);

            var invoiceInfoWindow = new PackingListWindow(invoice)
            {
                Owner = this
            };
            invoiceInfoWindow.Show();
        }   
        private void PaymentInvoiceOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var invoice = unitOfWork.Invoices.Get((int)button.Tag);

            var paymentWindow = new PaymentWindow(invoice)
            {
                Owner = this
            };
            paymentWindow.Show();
        }
        private void PartnerPaymentOnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.DataContext is PartnerPayment partnerPayment)
                {
                    if (checkBox.IsChecked == true)
                    {
                        if (partnerPayment.PaymentAmountIn == 0)
                        {
                            partnerPayment.IsActive = false;
                            checkBox.IsChecked = false;
                            var dialogWindow = new SmallDialogWindow("Нехитруй");
                            dialogWindow.ShowDialog();
                            return;
                        }
                        else
                        {
                            partnerPayment.IsActive = true;
                        }
                    }
                    else
                    {
                        partnerPayment.IsActive = false;
                    }
                    unitOfWork.PartnerPayments.Update(partnerPayment);
                    unitOfWork.Save();
                    Refresh();
                }
            }
        }      
        private void BillInvoiceOnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.DataContext is Invoice invoice)
                {
                    if (checkBox.IsChecked == true)
                    {
                        invoice.IsBill = true;
                    }
                    else 
                    { 
                        invoice.IsBill = false; 
                    }
                    unitOfWork.Invoices.Update(invoice);
                    unitOfWork.Save();
                    Refresh();
                }
            }
        }
        private void PartnerInvoiceOnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.DataContext is Invoice invoice)
                {
                    if (checkBox.IsChecked == true)
                    {
                        invoice.IsPartnerPayed = true;
                    }
                    else
                    {
                        invoice.IsPartnerPayed = false;
                    }
                    unitOfWork.Invoices.Update(invoice);
                    unitOfWork.Save();
                    Refresh();
                }
            }
        }

        private void Restore(string path)
        {
            if (path != null)
            {
                if (BackupHelper.AskRestore(Path.GetFileName(path)) == true)
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
                        StatusTextBlock.Foreground = Brushes.DarkGreen;
                    };

                    try
                    {
                        server.KillAllProcesses(dbName);
                        restore.SqlRestore(server);
                    }
                    catch 
                    {
                        StatusTextBlock.Text = "Помилка завантаження резервної копії!";
                        StatusTextBlock.Foreground = Brushes.DarkRed;
                    }

                    unitOfWork.Reload();
                    unitOfWork.Db.Invoices.Include(item => item.Car).Include(item => item.InvoiceParts).Include(item => item.Payments).Load();
                    DatabaseInvoices = unitOfWork.Db.Invoices.Local;
                    DatabaseInvoices.CollectionChanged += delegate
                    {
                        Refresh();
                    };
                    Refresh();
                }
            }
            else
                StatusTextBlock.Text = "";
        }

        private void Refresh()
        {
            OnPropertyChanged("LocalInvoices");
            OnPropertyChanged("MyInvoices");
            OnPropertyChanged("PartnerPayments");
            DataGridPartnerPayments.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataGridPartnerPayments.Items.SortDescriptions.Clear();
            DataGridPartnerPayments.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
            DataGridInvoices.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataGridInvoices.Items.SortDescriptions.Clear();
            DataGridInvoices.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
            DataGridMyInvoices.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataGridMyInvoices.Items.SortDescriptions.Clear();
            DataGridMyInvoices.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
            DataGridPaymentReport.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            OnPropertyChanged("PartnerInvoices");
            OnPropertyChanged("UnpayedInvoices");
            OnPropertyChanged("PayedInvoices");
            OnPropertyChanged("ReportInvoices");
            OnPropertyChanged("UnpayedInvoicesResidueSum");
            OnPropertyChanged("UnpayedInvoicesSumInDelivery");
            OnPropertyChanged("PartnerInvoicesPartnerSum");
            OnPropertyChanged("PaymentAmountOutSum");
        }
    }
}
