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

    public class ManufacturerRepository : IRepository<Manufacturer>
    {
        private DataContext db;

        public ManufacturerRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Manufacturer> GetAll()
        {
            return db.Manufacturers
                .Include(item => item.AdditionalInfos);
        }

        public Manufacturer Get(int id)
        {
            return db.Manufacturers.Find(id);
        }

        public IEnumerable<Manufacturer> Find(Func<Manufacturer, bool> predicate)
        {
            return db.Manufacturers
                .Include(item => item.AdditionalInfos)
                .Where(predicate);
        }

        public void Create(Manufacturer item)
        {
            db.Manufacturers.Add(item);
        }

        public void Update(Manufacturer item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Manufacturers.Find(id);
            if (item != null)
                db.Manufacturers.Remove(item);
        }
    }
}
