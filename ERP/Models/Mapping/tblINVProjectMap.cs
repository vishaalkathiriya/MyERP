using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVProjectMap : EntityTypeConfiguration<tblINVProject>
    {
        public tblINVProjectMap()
        {
            // Primary Key
            this.HasKey(t => t.PKProjectId);

            // Properties
            this.Property(t => t.ProjectTitle)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("tblINVProject");
            this.Property(t => t.PKProjectId).HasColumnName("PKProjectId");
            this.Property(t => t.FKInquiryId).HasColumnName("FKInquiryId");
            this.Property(t => t.ProjectTitle).HasColumnName("ProjectTitle");
            this.Property(t => t.ProjectType).HasColumnName("ProjectType");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.ProjectStatus).HasColumnName("ProjectStatus");
            this.Property(t => t.TotalHours).HasColumnName("TotalHours");
            this.Property(t => t.Currency).HasColumnName("Currency");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.Remarks).HasColumnName("Remarks");

            // Relationships
            this.HasRequired(t => t.tblINVInquiry)
                .WithMany(t => t.tblINVProjects)
                .HasForeignKey(d => d.FKInquiryId);

        }
    }
}
