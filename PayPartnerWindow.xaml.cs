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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for PayPartnerWindow.xaml
    /// </summary>
    public partial class PayPartnerWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isPartnerPayOnlyPayed;
        private decimal paySum;
        private decimal sum;
        private decimal unpayedSum;

        public IEnumerable<Invoice> Invoices => DatabaseInvoices.Where(item => !item.IsMine && !item.IsBill);
        public IEnumerable<Invoice> DatabaseInvoices;
        public IEnumerable<Invoice> PayedInvoices => Invoices.Where(item => !item.IsPartnerPayed && item.IsPayed);
        public IEnumerable<Invoice> UnpayedInvoices => Invoices.Where(item => !item.IsPartnerPayed && !item.IsPayed);
        public IEnumerable<Invoice> PartnerInvoices => Invoices.Where(item => !item.IsPartnerPayed);
        public decimal PartnerInvoicesPartnerSum => PartnerInvoices.Sum(item => item.PartnerUnpayed);
        public decimal PayedInvoicesPartnerSum => PayedInvoices.Sum(item => item.PartnerUnpayed);
        public bool IsPartnerPayOnlyPayed
        {
            get { return isPartnerPayOnlyPayed; }
            set
            {
                isPartnerPayOnlyPayed = value;
                OnPropertyChanged("IsPartnerPayUnpayed");
            }
        }
        public decimal PaySum
        {
            get { return paySum; }
            set
            {
                paySum = value;
                OnPropertyChanged("PaySum");
            }
        }
        public decimal UnpayedSum
        {
            get { return unpayedSum; }
            set
            {
                unpayedSum = value;
                OnPropertyChanged("UnpayedSum");
            }
        }
        public decimal Sum
        {
            get { return sum; }
            set
            {
                sum = value;
                OnPropertyChanged("Sum");
            }
        }

        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");
        public PayPartnerWindow()
        {
            InitializeComponent();
            PropertyChanged += EventHandlers;
            DatabaseInvoices = unitOfWork.Invoices.GetAll();
            DataContext = this;
            IsPartnerPayOnlyPayed = true;
            PaySum = 0;
            SetHandlers();
        }
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void EventHandlers(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PaySum")
            {
                Sum = PayedInvoicesPartnerSum - PaySum;
                UnpayedSum = PartnerInvoicesPartnerSum - PaySum;
            }
        }
        public void SetHandlers()
        {
            PayButton.Click += delegate
            {
                var date = DateTime.Now.ToString(" yyyy-MM-dd HH-mm-ss");
                JsonBackupHelper.Backup($"jsonBackup{date}");
                if (PaySum > PartnerInvoicesPartnerSum)
                {
                    string message = $"Сума партнерам: {PaySum:C2} \nперевищує рахунок по всім накладним: {PartnerInvoicesPartnerSum:C2}\nЗмініть суму";

                    SmallDialogWindow dialogWindow = new SmallDialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    return;
                }
                if (PaySum > PayedInvoicesPartnerSum && IsPartnerPayOnlyPayed)
                {
                    string message = $"Сума партнерам: {PaySum:C2} \nперевищує рахунок по сплаченим накладним: {PayedInvoicesPartnerSum:C2}\nЗмініть суму чи натисніть галочку";

                    SmallDialogWindow dialogWindow = new SmallDialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    return;
                }
                else
                {
                    var payedInvoices = PayedInvoices.ToList();
                    var leftover = PaySum;
                    payedInvoices.Sort((x, y) => x.Id.CompareTo(y.Id));
                    for (int i = 0; i < payedInvoices.Count && leftover >= 0; i++)
                    {
                        if (leftover >= payedInvoices[i].PartnerUnpayed)
                        {
                            leftover -= payedInvoices[i].PartnerUnpayed;
                            payedInvoices[i].PartnerPayed = payedInvoices[i].PartnerSum;
                            payedInvoices[i].IsPartnerPayed = true;
                            unitOfWork.Invoices.Update(payedInvoices[i]);
                        }
                        else
                        {
                            payedInvoices[i].PartnerPayed += leftover;
                            leftover = 0;
                            unitOfWork.Invoices.Update(payedInvoices[i]);
                        }
                    }

                    if(!IsPartnerPayOnlyPayed)
                    {
                        var unpayedInvoices = UnpayedInvoices.ToList();
                        unpayedInvoices.Sort((x, y) => x.Id.CompareTo(y.Id));
                        for (int i = 0; i < unpayedInvoices.Count && leftover >= 0; i++)
                        {
                            if (leftover >= unpayedInvoices[i].PartnerUnpayed)
                            {
                                leftover -= unpayedInvoices[i].PartnerUnpayed;
                                unpayedInvoices[i].PartnerPayed = unpayedInvoices[i].PartnerSum;
                                unpayedInvoices[i].IsPartnerPayed = true;
                                unitOfWork.Invoices.Update(unpayedInvoices[i]);
                            }
                            else
                            {
                                unpayedInvoices[i].PartnerPayed += leftover;
                                leftover = 0;
                                unitOfWork.Invoices.Update(unpayedInvoices[i]);
                            }
                        }
                    }
                    unitOfWork.Save();

                    if (leftover > 0)
                    {
                        string message2 = $"!";
                    }

                    string message = $"Суму партнерам успішно заплачено!";

                    SmallDialogWindow dialogWindow = new SmallDialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    Close();
                }

            };
            PaySumBox.LostFocus += delegate
            {
                OnPropertyChanged("PaySum");
            };
            
        }
    }
}
