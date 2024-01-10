using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;

namespace PartsManager.Model.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private DataContext db;
        private CarRepository carRepository;
        private InvoiceRepository invoiceRepository;
        private InvoicePartRepository invoicePartRepository;
        private MarkRepository markRepository;
        private ModelRepository modelRepository;
        private PartRepository partRepository;
        private PartTypeRepository partTypeRepository;
        private PaymentRepository paymentRepository;
        private AdditionalInfoRepository additionalInfoRepository;
        private ApiStandardRepository apiStandardRepository;
        private ManufacturerStandardRepository manufacturerStandardRepository;
        private ManufacturerRepository manufacturerRepository;
        private SaeQualityStandardRepository saeQualityStandardRepository;
        private PartApiStandardRepository partApiStandardRepository;
        private PartManufacturerStandardRepository partManufacturerStandardRepository;

        public DataContext Db { get { return db; } }

        public EFUnitOfWork(string connectionString)
        {
            db = DataContext.GetDataContext(connectionString);
        }

        public IRepository<Car> Cars 
            => carRepository ?? (carRepository = new CarRepository(db));
        public IRepository<Invoice> Invoices 
            => invoiceRepository ?? (invoiceRepository = new InvoiceRepository(db));
        public IRepository<InvoicePart> InvoiceParts
            => invoicePartRepository ?? (invoicePartRepository = new InvoicePartRepository(db));
        public IRepository<Mark> Marks 
            => markRepository ?? (markRepository = new MarkRepository(db));
        public IRepository<Entities.Model> Models
            => modelRepository ?? (modelRepository = new ModelRepository(db));
        public IRepository<Part> Parts
            => partRepository ?? (partRepository = new PartRepository(db));
        public IRepository<PartType> PartTypes
            => partTypeRepository ?? (partTypeRepository = new PartTypeRepository(db));
        public IRepository<Payment> Payments
            => paymentRepository ?? (paymentRepository = new PaymentRepository(db));
        public IRepository<AdditionalInfo> AdditionalInfos
            => additionalInfoRepository ?? (additionalInfoRepository = new AdditionalInfoRepository(db));
        public IRepository<ApiStandard> ApiStandards
            => apiStandardRepository ?? (apiStandardRepository = new ApiStandardRepository(db));
        public IRepository<ManufacturerStandard> ManufacturerStandards
            => manufacturerStandardRepository ?? (manufacturerStandardRepository = new ManufacturerStandardRepository(db));
        public IRepository<Manufacturer> Manufacturers
            => manufacturerRepository ?? (manufacturerRepository = new ManufacturerRepository(db));
        public IRepository<SaeQualityStandard> SaeQualityStandards
            => saeQualityStandardRepository ?? (saeQualityStandardRepository = new SaeQualityStandardRepository(db));
        public IRepository<PartApiStandard> PartApiStandards
            => partApiStandardRepository ?? (partApiStandardRepository = new PartApiStandardRepository(db));
        public IRepository<PartManufacturerStandard> PartManufacturerStandards
            => partManufacturerStandardRepository ?? (partManufacturerStandardRepository = new PartManufacturerStandardRepository(db));

        public void Save()
        {
            db.SaveChanges();
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}