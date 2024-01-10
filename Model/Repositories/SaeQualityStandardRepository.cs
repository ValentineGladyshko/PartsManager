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
    public class SaeQualityStandardRepository : IRepository<SaeQualityStandard>
    {
        private DataContext db;

        public SaeQualityStandardRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<SaeQualityStandard> GetAll()
        {
            return db.SaeQualityStandards
                .Include(item => item.AdditionalInfos);
        }

        public SaeQualityStandard Get(int id)
        {
            return db.SaeQualityStandards.Find(id);
        }

        public IEnumerable<SaeQualityStandard> Find(Func<SaeQualityStandard, bool> predicate)
        {
            return db.SaeQualityStandards
                .Include(item => item.AdditionalInfos)
                .Where(predicate);
        }

        public void Create(SaeQualityStandard item)
        {
            db.SaeQualityStandards.Add(item);
        }

        public void Update(SaeQualityStandard item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.SaeQualityStandards.Find(id);
            if (item != null)
                db.SaeQualityStandards.Remove(item);
        }
    }
}
