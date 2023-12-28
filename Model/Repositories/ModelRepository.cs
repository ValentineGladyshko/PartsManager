using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PartsManager.Model.Repositories
{
    public class ModelRepository : IRepository<Entities.Model>
    {
        private DataContext db;

        public ModelRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Entities.Model> GetAll()
        {
            return db.Models
                .Include(item => item.Mark)
                .Include(item => item.Cars);
        }

        public Entities.Model Get(int id)
        {
            return db.Models.Find(id);
        }

        public IEnumerable<Entities.Model> Find(Func<Entities.Model, bool> predicate)
        {
            return db.Models
                .Include(item => item.Mark)
                .Include(item => item.Cars)
                .Where(predicate);
        }

        public void Create(Entities.Model item)
        {
            db.Models.Add(item);
        }

        public void Update(Entities.Model item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Models.Find(id);
            if (item != null)
                db.Models.Remove(item);
        }
    }
}