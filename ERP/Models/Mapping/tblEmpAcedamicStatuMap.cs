using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpAcedamicStatuMap : EntityTypeConfiguration<tblEmpAcedamicStatu>
    {
        public tblEmpAcedamicStatuMap()
        {
            // Primary Key
            this.HasKey(t => t.AcedamicStatusId);

            // Properties
            this.Property(t => t.AcedamicStatus)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblEmpAcedamicStatus");
            this.Property(t => t.AcedamicStatusId).HasColumnName("AcedamicStatusId");
            this.Property(t => t.AcedamicStatus).HasColumnName("AcedamicStatus");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
