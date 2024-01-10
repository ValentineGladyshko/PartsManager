using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Repositories
{
    public class InvoiceRepository : IRepository<Invoice>
    {
        private DataContext db;

        public InvoiceRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Invoice> GetAll()
        {
            return db.Invoices
                .Include(item => item.Car)
                .Include(item => item.InvoiceParts)
                .Include(item => item.Payments);
        }

        public Invoice Get(int id)
        {
            return db.Invoices.Find(id);
        }

        public IEnumerable<Invoice> Find(Func<Invoice, bool> predicate)
        {
            return db.Invoices
                .Include(item => item.Car)
                .Include(item => item.InvoiceParts)
                .Include(item => item.Payments)
                .Where(predicate);
        }

        public void Create(Invoice item)
        {
            db.Invoices.Add(item);
        }

        public void Update(Invoice item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Invoices.Find(id);
            if (item != null)
                db.Invoices.Remove(item);
        }
    }
}
