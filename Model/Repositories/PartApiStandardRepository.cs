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
    public class PartApiStandardRepository : IRepository<PartApiStandard>
    {
        private DataContext db;

        public PartApiStandardRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<PartApiStandard> GetAll()
        {
            return db.PartApiStandards
                .Include(item => item.ApiStandard)
                .Include(item => item.Part);
        }

        public PartApiStandard Get(int id)
        {
            return db.PartApiStandards.Find(id);
        }

        public IEnumerable<PartApiStandard> Find(Func<PartApiStandard, bool> predicate)
        {
            return db.PartApiStandards
                .Include(item => item.ApiStandard)
                .Include(item => item.Part)
                .Where(predicate);
        }

        public void Create(PartApiStandard item)
        {
            db.PartApiStandards.Add(item);
        }

        public void Update(PartApiStandard item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.PartApiStandards.Find(id);
            if (item != null)
                db.PartApiStandards.Remove(item);
        }
    }
}
