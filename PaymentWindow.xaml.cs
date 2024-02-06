using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Invoice LocalInvoice { get; set; }
        private Payment localPayment;
        private ICollection<Payment> localPayments;
        public Payment LocalPayment 
        {
            get { return localPayment; }
            set
            {
                localPayment = value;
                OnPropertyChanged("LocalPayment");
            }
        }
        public ICollection<Payment> LocalPayments 
        {
            get { return localPayments; }
            set
            {
                localPayments = value;
                OnPropertyChanged("LocalPayments");
            }
        }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

        public PaymentWindow(Invoice localInvoice)
        {
            InitializeComponent();
            LocalInvoice = localInvoice;
            LocalPayments = LocalInvoice.Payments;
            LocalPayment = new Payment
            {
                Invoice = LocalInvoice,
                Date = DateTime.Now,
                Info = string.Empty,
            };
            DataContext = this;
            SetHandlers();
        }

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void SetHandlers()
        {
            CreatePaymentButton.Click += (object sender, RoutedEventArgs e) =>
            {
                if (LocalPayment.PaymentAmount <= 0)
                {
                    return;
                }
                unitOfWork.Payments.Create(LocalPayment);
                unitOfWork.Save();
                if (LocalInvoice.Residue == 0)
                {
                    LocalInvoice.IsPayed = true;
                    unitOfWork.Invoices.Update(LocalInvoice);
                    unitOfWork.Save();
                }
                LocalPayment = new Payment()
                {
                    Invoice = LocalInvoice,
                    Date = DateTime.Now,
                    Info = string.Empty,
                };
                LocalPayments = LocalInvoice.Payments.ToList();
                ResidueBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            };
            
        }

        public void DeletePaymentOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

            var paymentToDelete = unitOfWork.Payments.Get((int)button.Tag);

            string message = $"Ви впевнені що хочете видалити платіж сумою {paymentToDelete.PaymentAmount:C2} з накладної?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.Payments.Delete(paymentToDelete.Id);
            unitOfWork.Save();
            LocalPayments = LocalInvoice.Payments.ToList();
            ResidueBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
