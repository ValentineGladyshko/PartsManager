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