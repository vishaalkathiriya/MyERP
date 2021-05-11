using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpDocumentMap : EntityTypeConfiguration<tblEmpDocument>
    {
        public tblEmpDocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblEmpDocument");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.DocumentId).HasColumnName("DocumentId");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblDocument)
                .WithMany(t => t.tblEmpDocuments)
                .HasForeignKey(d => d.DocumentId);
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblEmpDocuments)
                .HasForeignKey(d => d.EmployeeId);

        }
    }
}
