using System;
using PartsManager.Model.Entities;

namespace PartsManager.Model.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Car> Cars { get; }
        IRepository<Invoice> Invoices { get; }
        IRepository<InvoicePart> InvoiceParts { get; }
        IRepository<Mark> Marks { get; }
        IRepository<Entities.Model> Models { get; }
        IRepository<Part> Parts { get; }
        IRepository<PartType> PartTypes { get; }
        void Save();
    }
}