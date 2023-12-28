using PartsManager.Model.Context;
using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PartsManager.Model.Repositories
{
    public class CarRepository : IRepository<Car>
    {
        private DataContext db;

        public CarRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Car> GetAll()
        {
            return db.Cars
                .Include(item => item.Model)
                .Include(item => item.Invoices);
        }

        public Car Get(int id)
        {
            return db.Cars.Find(id);
        }

        public IEnumerable<Car> Find(Func<Car, bool> predicate)
        {
            return db.Cars
                .Include(item => item.Model)
                .Include(item => item.Invoices)
                .Where(predicate);
        }

        public void Create(Car item)
        {
            db.Cars.Add(item);
        }

        public void Update(Car item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Cars.Find(id);
            if (item != null)
                db.Cars.Remove(item);
        }
    }
}