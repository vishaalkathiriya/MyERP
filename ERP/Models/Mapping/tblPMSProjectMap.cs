using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblPMSProjectMap : EntityTypeConfiguration<tblPMSProject>
    {
        public tblPMSProjectMap()
        {
            // Primary Key
            this.HasKey(t => t.ProjectId);

            // Properties
            this.Property(t => t.ProjectName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.TechnologiesId)
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("tblPMSProject");
            this.Property(t => t.ProjectId).HasColumnName("ProjectId");
            this.Property(t => t.ProjectName).HasColumnName("ProjectName");
            this.Property(t => t.TechnologiesId).HasColumnName("TechnologiesId");
            this.Property(t => t.ProjectType).HasColumnName("ProjectType");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.TotalEstDays).HasColumnName("TotalEstDays");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsArchived).HasColumnName("IsArchived");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
