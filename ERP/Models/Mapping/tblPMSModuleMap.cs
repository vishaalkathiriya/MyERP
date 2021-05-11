using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblPMSModuleMap : EntityTypeConfiguration<tblPMSModule>
    {
        public tblPMSModuleMap()
        {
            // Primary Key
            this.HasKey(t => t.ModuleId);

            // Properties
            this.Property(t => t.ModuleName)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblPMSModule");
            this.Property(t => t.ModuleId).HasColumnName("ModuleId");
            this.Property(t => t.ProjectId).HasColumnName("ProjectId");
            this.Property(t => t.ModuleName).HasColumnName("ModuleName");
            this.Property(t => t.ModuleType).HasColumnName("ModuleType");
            this.Property(t => t.Priority).HasColumnName("Priority");
            this.Property(t => t.IsArchived).HasColumnName("IsArchived");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
