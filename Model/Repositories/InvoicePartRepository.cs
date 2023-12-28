using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PartsManager.Model.Repositories
{
    public class InvoicePartRepository : IRepository<InvoicePart>
    {
        private DataContext db;

        public InvoicePartRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<InvoicePart> GetAll()
        {
            return db.InvoiceParts
                //.Include(item => item.Invoice)
                .Include(item => item.Part);
        }

        public InvoicePart Get(int id)
        {
            return db.InvoiceParts.Find(id);
        }

        public IEnumerable<InvoicePart> Find(Func<InvoicePart, bool> predicate)
        {
            return db.InvoiceParts
                //.Include(item => item.Invoice)
                .Include(item => item.Part)
                .Where(predicate);
        }

        public void Create(InvoicePart item)
        {
            db.InvoiceParts.Add(item);
        }

        public void Update(InvoicePart item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.InvoiceParts.Find(id);
            if (item != null)
                db.InvoiceParts.Remove(item);
        }
    }
}