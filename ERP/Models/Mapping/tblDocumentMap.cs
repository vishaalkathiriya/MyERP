using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblDocumentMap : EntityTypeConfiguration<tblDocument>
    {
        public tblDocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Documents)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblDocuments");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Documents).HasColumnName("Documents");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.DocumentTypeId).HasColumnName("DocumentTypeId");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
