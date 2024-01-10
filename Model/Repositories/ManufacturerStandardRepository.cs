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
    public class ManufacturerStandardRepository : IRepository<ManufacturerStandard>
    {
        private DataContext db;

        public ManufacturerStandardRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<ManufacturerStandard> GetAll()
        {
            return db.ManufacturerStandards
                .Include(item => item.PartManufacturerStandards);
        }

        public ManufacturerStandard Get(int id)
        {
            return db.ManufacturerStandards.Find(id);
        }

        public IEnumerable<ManufacturerStandard> Find(Func<ManufacturerStandard, bool> predicate)
        {
            return db.ManufacturerStandards
                .Include(item => item.PartManufacturerStandards)
                .Where(predicate);
        }

        public void Create(ManufacturerStandard item)
        {
            db.ManufacturerStandards.Add(item);
        }

        public void Update(ManufacturerStandard item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.ManufacturerStandards.Find(id);
            if (item != null)
                db.ManufacturerStandards.Remove(item);
        }
    }
}
