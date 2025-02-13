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
    public class PartnerPaymentRepository : IRepository<PartnerPayment>
    {
        private DataContext db;

        public PartnerPaymentRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<PartnerPayment> GetAll()
        {
            return db.PartnerPayments;
        }

        public PartnerPayment Get(int id)
        {
            return db.PartnerPayments.Find(id);
        }

        public IEnumerable<PartnerPayment> Find(Func<PartnerPayment, bool> predicate)
        {
            return db.PartnerPayments
                .Where(predicate);
        }

        public void Create(PartnerPayment item)
        {
            db.PartnerPayments.Add(item);
        }

        public void Update(PartnerPayment item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.PartnerPayments.Find(id);
            if (item != null)
                db.PartnerPayments.Remove(item);
        }
    }
}
