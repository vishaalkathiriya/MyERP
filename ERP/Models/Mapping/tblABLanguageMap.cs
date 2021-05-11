using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblABLanguageMap : EntityTypeConfiguration<tblABLanguage>
    {
        public tblABLanguageMap()
        {
            // Primary Key
            this.HasKey(t => t.LangId);

            // Properties
            this.Property(t => t.Language)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("tblABLanguage");
            this.Property(t => t.LangId).HasColumnName("LangId");
            this.Property(t => t.Language).HasColumnName("Language");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
