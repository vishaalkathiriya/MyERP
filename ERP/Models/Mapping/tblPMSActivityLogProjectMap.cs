using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblPMSActivityLogProjectMap : EntityTypeConfiguration<tblPMSActivityLogProject>
    {
        public tblPMSActivityLogProjectMap()
        {
            // Primary Key
            this.HasKey(t => t.ProjectLogId);

            // Properties
            this.Property(t => t.ProjectName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.DBAction)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("tblPMSActivityLogProject");
            this.Property(t => t.ProjectLogId).HasColumnName("ProjectLogId");
            this.Property(t => t.ProjectId).HasColumnName("ProjectId");
            this.Property(t => t.ProjectName).HasColumnName("ProjectName");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.TotalEstDays).HasColumnName("TotalEstDays");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsArchived).HasColumnName("IsArchived");
            this.Property(t => t.DBAction).HasColumnName("DBAction");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
        }
    }
}
