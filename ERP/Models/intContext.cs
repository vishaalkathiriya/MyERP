using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using ERP.Models.Mapping;

namespace ERP.Models
{
    public partial class intContext : DbContext
    {
        static intContext()
        {
            Database.SetInitializer<intContext>(null);
        }

        public intContext()
            : base("Name=intContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<VisitorInOut> VisitorInOuts { get; set; }
        public DbSet<VisitorMaster> VisitorMasters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new VisitorInOutMap());
            modelBuilder.Configurations.Add(new VisitorMasterMap());
        }
    }
}
