using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblSRParameterMap : EntityTypeConfiguration<tblSRParameter>
    {
        public tblSRParameterMap()
        {
            // Primary Key
            this.HasKey(t => t.ParameterId);

            // Properties
            this.Property(t => t.ParameterName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblSRParameter");
            this.Property(t => t.ParameterId).HasColumnName("ParameterId");
            this.Property(t => t.SubTypeId).HasColumnName("SubTypeId");
            this.Property(t => t.ParameterName).HasColumnName("ParameterName");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblSRSubType)
                .WithMany(t => t.tblSRParameters)
                .HasForeignKey(d => d.SubTypeId);

        }
    }
}
