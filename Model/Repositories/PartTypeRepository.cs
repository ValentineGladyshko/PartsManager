using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PartsManager.Model.Repositories
{
    public class PartTypeRepository : IRepository<PartType>
    {
        private DataContext db;

        public PartTypeRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<PartType> GetAll()
        {
            return db.PartTypes
                .Include(item => item.Parts);
        }

        public PartType Get(int id)
        {
            return db.PartTypes.Find(id);
        }

        public IEnumerable<PartType> Find(Func<PartType, bool> predicate)
        {
            return db.PartTypes
                .Include(item => item.Parts)
                .Where(predicate);
        }

        public void Create(PartType item)
        {
            db.PartTypes.Add(item);
        }

        public void Update(PartType item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.PartTypes.Find(id);
            if (item != null)
                db.PartTypes.Remove(item);
        }
    }
}