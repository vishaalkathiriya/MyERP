using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblPMSActivityLogProjectUserMap : EntityTypeConfiguration<tblPMSActivityLogProjectUser>
    {
        public tblPMSActivityLogProjectUserMap()
        {
            // Primary Key
            this.HasKey(t => t.ProjectUserLogId);

            // Properties
            this.Property(t => t.DBAction)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("tblPMSActivityLogProjectUser");
            this.Property(t => t.ProjectUserLogId).HasColumnName("ProjectUserLogId");
            this.Property(t => t.ProjectUserId).HasColumnName("ProjectUserId");
            this.Property(t => t.ProjectId).HasColumnName("ProjectId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.IsTL).HasColumnName("IsTL");
            this.Property(t => t.DBAction).HasColumnName("DBAction");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
        }
    }
}
