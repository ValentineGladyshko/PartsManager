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
    public class ApiStandardRepository : IRepository<ApiStandard>
    {
        private DataContext db;

        public ApiStandardRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<ApiStandard> GetAll()
        {
            return db.ApiStandards
                .Include(item => item.PartApiStandards);
        }

        public ApiStandard Get(int id)
        {
            return db.ApiStandards.Find(id);
        }

        public IEnumerable<ApiStandard> Find(Func<ApiStandard, bool> predicate)
        {
            return db.ApiStandards
                .Include(item => item.PartApiStandards)
                .Where(predicate);
        }

        public void Create(ApiStandard item)
        {
            db.ApiStandards.Add(item);
        }

        public void Update(ApiStandard item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.ApiStandards.Find(id);
            if (item != null)
                db.ApiStandards.Remove(item);
        }
    }
}
