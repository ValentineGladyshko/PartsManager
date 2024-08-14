using PartsManager.Model.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;

namespace PartsManager.Model.Context
{
    public class JsonDatabase
    {
        public Dictionary<int, Car> Cars { get; set; }
        public Dictionary<int, Invoice> Invoices { get; set; }
        public Dictionary<int, InvoicePart> InvoiceParts { get; set; }
        public Dictionary<int, Mark> Marks { get; set; }
        public Dictionary<int, Entities.Model> Models { get; set; }
        public Dictionary<int, Part> Parts { get; set; }
        public Dictionary<int, PartType> PartTypes { get; set; }
        public Dictionary<int, Payment> Payments { get; set; }
        public Dictionary<int, ApiStandard> ApiStandards { get; set; }
        public Dictionary<int, SaeQualityStandard> SaeQualityStandards { get; set; }
        public Dictionary<int, Manufacturer> Manufacturers { get; set; }
        public Dictionary<int, ManufacturerStandard> ManufacturerStandards { get; set; }
        public Dictionary<int, AdditionalInfo> AdditionalInfos { get; set; }
        public Dictionary<int, PartApiStandard> PartApiStandards { get; set; }
        public Dictionary<int, PartManufacturerStandard> PartManufacturerStandards { get; set; }

        public JsonDatabase()
        {
            Marks = new Dictionary<int, Mark>();
            Models = new Dictionary<int, Entities.Model>();
            Cars = new Dictionary<int, Car>();
            PartTypes = new Dictionary<int, PartType>();
            Parts = new Dictionary<int, Part>();
            Invoices = new Dictionary<int, Invoice>();
            InvoiceParts = new Dictionary<int, InvoicePart>();
            Payments = new Dictionary<int, Payment>();
            ApiStandards = new Dictionary<int, ApiStandard>();
            Manufacturers = new Dictionary<int, Manufacturer>();
            ManufacturerStandards = new Dictionary<int, ManufacturerStandard>();
            SaeQualityStandards = new Dictionary<int, SaeQualityStandard>();
            AdditionalInfos = new Dictionary<int, AdditionalInfo>();
            PartApiStandards = new Dictionary<int, PartApiStandard>();
            PartManufacturerStandards = new Dictionary<int, PartManufacturerStandard>();
        }
        public JsonDatabase(DatabaseBackup databaseBackup)
        {
            Marks = databaseBackup.Marks.ToDictionary(item => item.Id, item => item);
            Models = databaseBackup.Models.ToDictionary(item => item.Id, item => item);
            Cars = databaseBackup.Cars.ToDictionary(item => item.Id, item => item);
            PartTypes = databaseBackup.PartTypes.ToDictionary(item => item.Id, item => item);
            Parts = databaseBackup.Parts.ToDictionary(item => item.Id, item => item);
            Invoices = databaseBackup.Invoices.ToDictionary(item => item.Id, item => item);
            InvoiceParts = databaseBackup.InvoiceParts.ToDictionary(item => item.Id, item => item);
            Payments = databaseBackup.Payments.ToDictionary(item => item.Id, item => item);
            ApiStandards = databaseBackup.ApiStandards.ToDictionary(item => item.Id, item => item);
            Manufacturers = databaseBackup.Manufacturers.ToDictionary(item => item.Id, item => item);
            ManufacturerStandards = databaseBackup.ManufacturerStandards.ToDictionary(item => item.Id, item => item);
            SaeQualityStandards = databaseBackup.SaeQualityStandards.ToDictionary(item => item.Id, item => item);
            AdditionalInfos = databaseBackup.AdditionalInfos.ToDictionary(item => item.Id, item => item);
            PartApiStandards = databaseBackup.PartApiStandards.ToDictionary(item => item.Id, item => item);
            PartManufacturerStandards = databaseBackup.PartManufacturerStandards.ToDictionary(item => item.Id, item => item);
            LoadContext();
        }

        public void LoadContext()
        {
            foreach (var item in Models)
            {
                var mark = Marks[item.Value.MarkId];
                if (mark != null)
                {
                    item.Value.Mark = mark;
                    if (mark.Models == null)
                    {
                        mark.Models = new List<Entities.Model>();
                    }
                    mark.Models.Add(item.Value);
                }
            }

            foreach (var item in Cars)
            {
                var model = Models[item.Value.ModelId];
                if (model != null)
                {
                    item.Value.Model = model;
                    if (model.Cars == null)
                    {
                        model.Cars = new List<Car>();
                    }
                    model.Cars.Add(item.Value);
                }
            }

            foreach (var item in Parts)
            {
                var partType = PartTypes[item.Value.PartTypeId];
                if (partType != null)
                {
                    item.Value.PartType = partType;
                    if (partType.Parts == null)
                    {
                        partType.Parts = new List<Part>();
                    }
                    partType.Parts.Add(item.Value);
                }
            }

            foreach (var item in Invoices)
            {
                var car = Cars[item.Value.CarId];
                if (car != null)
                {
                    item.Value.Car = car;
                    if (car.Invoices == null)
                    {
                        car.Invoices = new List<Invoice>();
                    }
                    car.Invoices.Add(item.Value);
                }
            }

            foreach (var item in InvoiceParts)
            {
                var invoice = Invoices[item.Value.InvoiceId];
                if (invoice != null)
                {
                    item.Value.Invoice = invoice;
                    if (invoice.InvoiceParts == null)
                    {
                        invoice.InvoiceParts = new ObservableCollection<InvoicePart>();
                    }
                    invoice.InvoiceParts.Add(item.Value);
                }
                var part = Parts[item.Value.PartId];
                if (part != null)
                {
                    item.Value.Part = part;
                    if (part.InvoiceParts == null)
                    {
                        part.InvoiceParts = new List<InvoicePart>();
                    }
                    part.InvoiceParts.Add(item.Value);
                }
            }

            foreach (var item in Payments)
            {
                var invoice = Invoices[item.Value.InvoiceId];
                if (invoice != null)
                {
                    item.Value.Invoice = invoice;
                    if (invoice.Payments == null)
                    {
                        invoice.Payments = new List<Payment>();
                    }
                    invoice.Payments.Add(item.Value);
                }
            }

            foreach (var item in AdditionalInfos)
            {
                var part = Parts[item.Value.PartId];
                if (part != null)
                {
                    item.Value.Part = part;
                    if (part.AdditionalInfos == null)
                    {
                        part.AdditionalInfos = new List<AdditionalInfo>();
                    }
                    part.AdditionalInfos.Add(item.Value);
                }

                var saeQualityStandard = SaeQualityStandards[item.Value.SaeQualityStandardId];
                if (saeQualityStandard != null)
                {
                    item.Value.SaeQualityStandard = saeQualityStandard;
                    if (saeQualityStandard.AdditionalInfos == null)
                    {
                        saeQualityStandard.AdditionalInfos = new List<AdditionalInfo>();
                    }
                    saeQualityStandard.AdditionalInfos.Add(item.Value);
                }

                var manufacturer = Manufacturers[item.Value.ManufacturerId];
                if (manufacturer != null)
                {
                    item.Value.Manufacturer = manufacturer;
                    if (manufacturer.AdditionalInfos == null)
                    {
                        manufacturer.AdditionalInfos = new List<AdditionalInfo>();
                    }
                    manufacturer.AdditionalInfos.Add(item.Value);
                }
            }

            foreach (var item in PartApiStandards)
            {
                var part = Parts[item.Value.PartId];
                if (part != null)
                {
                    item.Value.Part = part;
                    if (part.PartApiStandards == null)
                    {
                        part.PartApiStandards = new List<PartApiStandard>();
                    }
                    part.PartApiStandards.Add(item.Value);
                }

                var apiStandard = ApiStandards[item.Value.ApiStandardId];
                if (apiStandard != null)
                {
                    item.Value.ApiStandard = apiStandard;
                    if (apiStandard.PartApiStandards == null)
                    {
                        apiStandard.PartApiStandards = new List<PartApiStandard>();
                    }
                    apiStandard.PartApiStandards.Add(item.Value);
                }
            }

            foreach (var item in PartManufacturerStandards)
            {
                var part = Parts[item.Value.PartId];
                if (part != null)
                {
                    item.Value.Part = part;
                    if (part.PartManufacturerStandards == null)
                    {
                        part.PartManufacturerStandards = new List<PartManufacturerStandard>();
                    }
                    part.PartManufacturerStandards.Add(item.Value);
                }

                var manufacturerStandard = ManufacturerStandards[item.Value.ManufacturerStandardId];
                if (manufacturerStandard != null)
                {
                    item.Value.ManufacturerStandard = manufacturerStandard;
                    if (manufacturerStandard.PartManufacturerStandards == null)
                    {
                        manufacturerStandard.PartManufacturerStandards = new List<PartManufacturerStandard>();
                    }
                    manufacturerStandard.PartManufacturerStandards.Add(item.Value);
                }
            }
        }

        public void RearrangeID(int invoiceLastCorrectId)
        {
            int currentID = 1;
            foreach (var item in Marks)
            {
                item.Value.Id = currentID++;
                if (item.Value.Models != null)
                {
                    foreach (var childItem in item.Value.Models)
                    {
                        childItem.MarkId = childItem.Mark.Id;
                    }
                }
            }

            currentID = 1;
            foreach (var item in Models)
            {
                item.Value.Id = currentID++;
                if (item.Value.Cars != null)
                {
                    foreach (var childItem in item.Value.Cars)
                    {
                        childItem.ModelId = childItem.Model.Id;
                    }
                }
            }

            currentID = 1;
            foreach (var item in Cars)
            {
                item.Value.Id = currentID++;
                if (item.Value.Invoices != null)
                {
                    foreach (var childItem in item.Value.Invoices)
                    {
                        childItem.CarId = childItem.Car.Id;
                    }
                }
            }

            currentID = 1;
            foreach (var item in PartTypes)
            {
                item.Value.Id = currentID++;
                if (item.Value.Parts != null)
                {
                    foreach (var childItem in item.Value.Parts)
                    {
                        childItem.PartTypeId = childItem.PartType.Id;
                    }
                }
            }

            currentID = 1;
            foreach (var item in Parts)
            {
                item.Value.Id = currentID++;
                if (item.Value.InvoiceParts != null)
                {
                    foreach (var childItem in item.Value.InvoiceParts)
                    {
                        childItem.PartId = childItem.Part.Id;
                    }
                }
                if (item.Value.AdditionalInfos != null)
                {
                    foreach (var childItem in item.Value.AdditionalInfos)
                    {
                        childItem.PartId = childItem.Part.Id;
                    }
                }
                if (item.Value.PartApiStandards != null)
                {
                    foreach (var childItem in item.Value.PartApiStandards)
                    {
                        childItem.PartId = childItem.Part.Id;
                    }
                }
                if (item.Value.PartManufacturerStandards != null)
                {
                    foreach (var childItem in item.Value.PartManufacturerStandards)
                    {
                        childItem.PartId = childItem.Part.Id;
                    }
                }
            }

            bool startChange = false;
            currentID = invoiceLastCorrectId;
            foreach (var item in Invoices)
            {
                if (startChange)
                {
                    item.Value.Id = ++currentID;
                    if (item.Value.InvoiceParts != null)
                    {
                        foreach (var childItem in item.Value.InvoiceParts)
                        {
                            childItem.InvoiceId = childItem.Invoice.Id;
                        }
                    }
                    if (item.Value.Payments != null)
                    {
                        foreach (var childItem in item.Value.Payments)
                        {
                            childItem.InvoiceId = childItem.Invoice.Id;
                        }
                    }
                }
                if (item.Key == invoiceLastCorrectId)
                    startChange = true;
            }

            currentID = 1;
            foreach (var item in InvoiceParts)
            {
                item.Value.Id = currentID++;
            }

            currentID = 1;
            foreach (var item in Payments)
            {
                item.Value.Id = currentID++;
            }

            currentID = 1;
            foreach (var item in ApiStandards)
            {
                item.Value.Id = currentID++;
                if (item.Value.PartApiStandards != null)
                {
                    foreach (var childItem in item.Value.PartApiStandards)
                    {
                        childItem.ApiStandardId = childItem.ApiStandard.Id;
                    }
                }
            }

            currentID = 1;
            foreach (var item in Manufacturers)
            {
                item.Value.Id = currentID++;
                if (item.Value.AdditionalInfos != null)
                {
                    foreach (var childItem in item.Value.AdditionalInfos)
                    {
                        childItem.ManufacturerId = childItem.Manufacturer.Id;
                    }
                }
            }

            currentID = 1;
            foreach (var item in ManufacturerStandards)
            {
                item.Value.Id = currentID++;
                if (item.Value.PartManufacturerStandards != null)
                {
                    foreach (var childItem in item.Value.PartManufacturerStandards)
                    {
                        childItem.ManufacturerStandardId = childItem.ManufacturerStandard.Id;
                    }
                }
            }

            currentID = 1;
            foreach (var item in SaeQualityStandards)
            {
                item.Value.Id = currentID++;
                if (item.Value.AdditionalInfos != null)
                {
                    foreach (var childItem in item.Value.AdditionalInfos)
                    {
                        childItem.SaeQualityStandardId = childItem.SaeQualityStandard.Id;
                    }
                }
            }

            currentID = 1;
            foreach (var item in AdditionalInfos)
            {
                item.Value.Id = currentID++;
            }

            currentID = 1;
            foreach (var item in PartApiStandards)
            {
                item.Value.Id = currentID++;
            }

            currentID = 1;
            foreach (var item in PartManufacturerStandards)
            {
                item.Value.Id = currentID++;
            }
        }
    }
}
