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
    public class PaymentRepository : IRepository<Payment>
    {
        private DataContext db;

        public PaymentRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Payment> GetAll()
        {
            return db.Payments
                .Include(item => item.Invoice);
        }

        public Payment Get(int id)
        {
            return db.Payments.Find(id);
        }

        public IEnumerable<Payment> Find(Func<Payment, bool> predicate)
        {
            return db.Payments
                .Include(item => item.Invoice)
                .Where(predicate);
        }

        public void Create(Payment item)
        {
            db.Payments.Add(item);
        }

        public void Update(Payment item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Payments.Find(id);
            if (item != null)
                db.Payments.Remove(item);
        }
    }
}
