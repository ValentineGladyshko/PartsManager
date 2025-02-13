using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class DatabaseBackup
    {
        public List<AdditionalInfo> AdditionalInfos { get; set; }
        public List<ApiStandard> ApiStandards { get; set; }
        public List<Car> Cars { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<InvoicePart> InvoiceParts { get; set; }
        public List<Manufacturer> Manufacturers { get; set; }
        public List<ManufacturerStandard> ManufacturerStandards { get; set; }
        public List<Mark> Marks { get; set; }
        public List<Model> Models { get; set; }
        public List<Part> Parts { get; set; }
        public List<PartApiStandard> PartApiStandards { get; set; }
        public List<PartManufacturerStandard> PartManufacturerStandards { get; set; }
        public List<PartType> PartTypes { get; set; }
        public List<Payment> Payments { get; set; }
        public List<SaeQualityStandard> SaeQualityStandards { get; set; }
        public List<PartnerPayment> PartnerPayments { get; set; }

        public DatabaseBackup() 
        {
            EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

            AdditionalInfos = unitOfWork.AdditionalInfos.GetAll().ToList();
            ApiStandards = unitOfWork.ApiStandards.GetAll().ToList();
            Cars = unitOfWork.Cars.GetAll().ToList();
            Invoices = unitOfWork.Invoices.GetAll().ToList();
            InvoiceParts = unitOfWork.InvoiceParts.GetAll().ToList();
            Manufacturers = unitOfWork.Manufacturers.GetAll().ToList();
            ManufacturerStandards = unitOfWork.ManufacturerStandards.GetAll().ToList();
            Marks = unitOfWork.Marks.GetAll().ToList();
            Models = unitOfWork.Models.GetAll().ToList();
            Parts = unitOfWork.Parts.GetAll().ToList();
            PartApiStandards = unitOfWork.PartApiStandards.GetAll().ToList();
            PartManufacturerStandards = unitOfWork.PartManufacturerStandards.GetAll().ToList();
            PartTypes = unitOfWork.PartTypes.GetAll().ToList();
            Payments = unitOfWork.Payments.GetAll().ToList();
            SaeQualityStandards = unitOfWork.SaeQualityStandards.GetAll().ToList();
            PartnerPayments = unitOfWork.PartnerPayments.GetAll().ToList();
        }

        public void LoadContext()
        {
            foreach(var t in Models)
            {
                t.Mark = Marks.First(item => item.Id == t.MarkId);
            }
        }
    }
}
