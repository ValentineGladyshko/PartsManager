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
    public class AdditionalInfoRepository : IRepository<AdditionalInfo>
    {
        private DataContext db;

        public AdditionalInfoRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<AdditionalInfo> GetAll()
        {
            return db.AdditionalInfos
                .Include(item => item.Part)
                .Include(item => item.SaeQualityStandard)
                .Include(item => item.Manufacturer);
        }

        public AdditionalInfo Get(int id)
        {
            return db.AdditionalInfos.Find(id);
        }

        public IEnumerable<AdditionalInfo> Find(Func<AdditionalInfo, bool> predicate)
        {
            return db.AdditionalInfos
                .Include(item => item.Part)
                .Include(item => item.SaeQualityStandard)
                .Include(item => item.Manufacturer)
                .Where(predicate);
        }

        public void Create(AdditionalInfo item)
        {
            db.AdditionalInfos.Add(item);
        }

        public void Update(AdditionalInfo item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.AdditionalInfos.Find(id);
            if (item != null)
                db.AdditionalInfos.Remove(item);
        }
    }
}
