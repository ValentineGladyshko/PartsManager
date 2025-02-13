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
        public PartnerPayment LocalPartnerPayment { get; set; }
        private bool isPartnerPaymentCreated;
        public bool IsPartnerPaymentCreated
        {
            get { return isPartnerPaymentCreated; }
            set
            {
                isPartnerPaymentCreated = value;
                OnPropertyChanged("IsPartnerPaymentCreated");
            }
        }
        private ActionType Action { get; set; }

        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");
        public PayPartnerWindow()
        {
            InitializeComponent();
            LocalPartnerPayment = new PartnerPayment()
            {
                DateIn = DateTime.Now,
                DateOut = DateTime.Now,
            };
            PropertyChanged += EventHandlers;
            IsPartnerPaymentCreated = false;
            DataContext = this;
            SetHandlers();
        }
        public PayPartnerWindow(PartnerPayment partnerPayment)
        {
            InitializeComponent();
            LocalPartnerPayment = partnerPayment;
            PropertyChanged += EventHandlers;
            IsPartnerPaymentCreated = true;
            DataContext = this;
            SetHandlers();
        }
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void EventHandlers(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsPartnerPaymentCreated")
            {
                if (!IsPartnerPaymentCreated)
                {
                    SaveButton.Content = "Створити платіж";
                    Action = ActionType.Create;
                }
                else
                {
                    SaveButton.Content = "Редагувати платіж";
                    Action = ActionType.Edit;                   
                }
            }
        }
        public void SetHandlers()
        {
            SaveButton.Click += delegate
            {
                if (LocalPartnerPayment.BackPayment > 0 && LocalPartnerPayment.InvoicePayment > 0)
                {
                    var dialogWindow = new SmallDialogWindow("Повернення коштів та Списання боргу більше за 0");
                    dialogWindow.ShowDialog();
                    return;
                }
                else if (LocalPartnerPayment.PaymentAmountIn < 0)
                {
                    var dialogWindow = new SmallDialogWindow("Розрахунок менше за 0");
                    dialogWindow.ShowDialog();
                    return;
                }
                else
                {
                    if (Action == ActionType.Create)
                    {
                        unitOfWork.PartnerPayments.Create(LocalPartnerPayment);
                        unitOfWork.Save();
                        IsPartnerPaymentCreated = true;
                        Close();
                    }
                    else if (Action == ActionType.Edit)
                    {
                        unitOfWork.PartnerPayments.Update(LocalPartnerPayment);
                        unitOfWork.Save();
                        Close();
                    }
                }
            };
        }
    }
}
