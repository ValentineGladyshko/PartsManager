using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PartsManager.Model.Repositories
{
    public class PartRepository : IRepository<Part>
    {
        private DataContext db;

        public PartRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Part> GetAll()
        {
            return db.Parts
                .Include(item => item.PartType)
                .Include(item => item.InvoiceParts);
        }

        public Part Get(int id)
        {
            return db.Parts.Find(id);
        }

        public IEnumerable<Part> Find(Func<Part, bool> predicate)
        {
            return db.Parts
                .Include(item => item.PartType)
                .Include(item => item.InvoiceParts)
                .Where(predicate);
        }

        public void Create(Part item)
        {
            db.Parts.Add(item);
        }

        public void Update(Part item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Parts.Find(id);
            if (item != null)
                db.Parts.Remove(item);
        }
    }
}