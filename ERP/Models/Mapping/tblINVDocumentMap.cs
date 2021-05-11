using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVDocumentMap : EntityTypeConfiguration<tblINVDocument>
    {
        public tblINVDocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.PKDocId);

            // Properties
            this.Property(t => t.DocName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Remarks)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("tblINVDocument");
            this.Property(t => t.PKDocId).HasColumnName("PKDocId");
            this.Property(t => t.tblRefId).HasColumnName("tblRefId");
            this.Property(t => t.DocId).HasColumnName("DocId");
            this.Property(t => t.DocName).HasColumnName("DocName");
            this.Property(t => t.DocTypeId).HasColumnName("DocTypeId");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
