using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblABTemplateMap : EntityTypeConfiguration<tblABTemplate>
    {
        public tblABTemplateMap()
        {
            // Primary Key
            this.HasKey(t => t.TemplateId);

            // Properties
            this.Property(t => t.TemplateName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TemplateFormate)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tblABTemplate");
            this.Property(t => t.TemplateId).HasColumnName("TemplateId");
            this.Property(t => t.TemplateName).HasColumnName("TemplateName");
            this.Property(t => t.TemplateFormate).HasColumnName("TemplateFormate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
