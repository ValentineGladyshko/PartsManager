using PartsManager.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartsManager.Model.Context
{
    public class DataContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePart> InvoiceParts { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Entities.Model> Models { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PartType> PartTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ApiStandard> ApiStandards { get; set; }
        public DbSet<SaeQualityStandard> SaeQualityStandards { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ManufacturerStandard> ManufacturerStandards { get; set; }
        public DbSet<AdditionalInfo> AdditionalInfos { get; set; }
        public DbSet<PartApiStandard> PartApiStandards { get; set; }
        public DbSet<PartManufacturerStandard> PartManufacturerStandards { get; set; }


        static DataContext()
        {
            Database.SetInitializer<DataContext>(new StoreDbInitializer());
        }

        private static DataContext dataContext;
        private DataContext(string connectionString) : base(connectionString)
        {
        }

        public static DataContext GetDataContext(string connectionString)
        {
            if (dataContext == null)
            {
                dataContext = new DataContext(connectionString);
            }

            return dataContext;
        }

        public class StoreDbInitializer : DropCreateDatabaseIfModelChanges<DataContext>
        {
            protected override void Seed(DataContext db)
            {
                db.SaveChanges();
            }
        }
    }
}
