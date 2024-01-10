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
    public class PartManufacturerStandardRepository : IRepository<PartManufacturerStandard>
    {
        private DataContext db;

        public PartManufacturerStandardRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<PartManufacturerStandard> GetAll()
        {
            return db.PartManufacturerStandards
                .Include(item => item.ManufacturerStandard)
                .Include(item => item.Part);
        }

        public PartManufacturerStandard Get(int id)
        {
            return db.PartManufacturerStandards.Find(id);
        }

        public IEnumerable<PartManufacturerStandard> Find(Func<PartManufacturerStandard, bool> predicate)
        {
            return db.PartManufacturerStandards
                .Include(item => item.ManufacturerStandard)
                .Include(item => item.Part)
                .Where(predicate);
        }

        public void Create(PartManufacturerStandard item)
        {
            db.PartManufacturerStandards.Add(item);
        }

        public void Update(PartManufacturerStandard item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.PartManufacturerStandards.Find(id);
            if (item != null)
                db.PartManufacturerStandards.Remove(item);
        }
    }
}
