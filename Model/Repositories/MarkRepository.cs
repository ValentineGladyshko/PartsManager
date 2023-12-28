using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PartsManager.Model.Repositories
{
    public class MarkRepository : IRepository<Mark>
    {
        private DataContext db;

        public MarkRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Mark> GetAll()
        {
            return db.Marks
                .Include(item => item.Models);
        }

        public Mark Get(int id)
        {
            return db.Marks.Find(id);
        }

        public IEnumerable<Mark> Find(Func<Mark, bool> predicate)
        {
            return db.Marks
                .Include(item => item.Models)
                .Where(predicate);
        }

        public void Create(Mark item)
        {
            db.Marks.Add(item);
        }

        public void Update(Mark item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Marks.Find(id);
            if (item != null)
                db.Marks.Remove(item);
        }
    }
}