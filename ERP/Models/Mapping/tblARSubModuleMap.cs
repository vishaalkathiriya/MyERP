using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblARSubModuleMap : EntityTypeConfiguration<tblARSubModule>
    {
        public tblARSubModuleMap()
        {
            // Primary Key
            this.HasKey(t => t.SubModuleId);

            // Properties
            this.Property(t => t.SubModuleName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.URL)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AllowedAccess)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblARSubModule");
            this.Property(t => t.SubModuleId).HasColumnName("SubModuleId");
            this.Property(t => t.ModuleId).HasColumnName("ModuleId");
            this.Property(t => t.SubModuleName).HasColumnName("SubModuleName");
            this.Property(t => t.URL).HasColumnName("URL");
            this.Property(t => t.AllowedAccess).HasColumnName("AllowedAccess");
            this.Property(t => t.SeqNo).HasColumnName("SeqNo");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblARModule)
                .WithMany(t => t.tblARSubModules)
                .HasForeignKey(d => d.ModuleId);

        }
    }
}
