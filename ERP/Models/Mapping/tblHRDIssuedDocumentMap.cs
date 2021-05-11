using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDIssuedDocumentMap : EntityTypeConfiguration<tblHRDIssuedDocument>
    {
        public tblHRDIssuedDocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.HRDIssuedDocId);

            // Properties
            this.Property(t => t.ECode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FullName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.AttachmentName)
                .HasMaxLength(50);

            this.Property(t => t.DepartmentName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IssuedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("tblHRDIssuedDocument");
            this.Property(t => t.HRDIssuedDocId).HasColumnName("HRDIssuedDocId");
            this.Property(t => t.ECode).HasColumnName("ECode");
            this.Property(t => t.FullName).HasColumnName("FullName");
            this.Property(t => t.DocumentTypeId).HasColumnName("DocumentTypeId");
            this.Property(t => t.FromDate).HasColumnName("FromDate");
            this.Property(t => t.ToDate).HasColumnName("ToDate");
            this.Property(t => t.AttachmentName).HasColumnName("AttachmentName");
            this.Property(t => t.DepartmentName).HasColumnName("DepartmentName");
            this.Property(t => t.IntercomNo).HasColumnName("IntercomNo");
            this.Property(t => t.IssuedBy).HasColumnName("IssuedBy");
            this.Property(t => t.IssuedOn).HasColumnName("IssuedOn");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblDocument)
                .WithMany(t => t.tblHRDIssuedDocuments)
                .HasForeignKey(d => d.DocumentTypeId);

        }
    }
}
