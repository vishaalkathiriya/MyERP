using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRTypeMap : EntityTypeConfiguration<tblSRType>
    {
        public tblSRTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.TypeId);

            // Properties
            this.Property(t => t.TypePrefix)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.TypeName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRType");
            this.Property(t => t.TypeId).HasColumnName("TypeId");
            this.Property(t => t.TypePrefix).HasColumnName("TypePrefix");
            this.Property(t => t.TypeName).HasColumnName("TypeName");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
