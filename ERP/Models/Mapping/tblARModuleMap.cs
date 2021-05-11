using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblARModuleMap : EntityTypeConfiguration<tblARModule>
    {
        public tblARModuleMap()
        {
            // Primary Key
            this.HasKey(t => t.ModuleId);

            // Properties
            this.Property(t => t.ModuleName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblARModules");
            this.Property(t => t.ModuleId).HasColumnName("ModuleId");
            this.Property(t => t.ModuleName).HasColumnName("ModuleName");
            this.Property(t => t.SeqNo).HasColumnName("SeqNo");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
