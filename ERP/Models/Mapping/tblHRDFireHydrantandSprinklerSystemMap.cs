using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDFireHydrantandSprinklerSystemMap : EntityTypeConfiguration<tblHRDFireHydrantandSprinklerSystem>
    {
        public tblHRDFireHydrantandSprinklerSystemMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.BuildingName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.CheckedBy)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Findings)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.RootCause)
                .HasMaxLength(250);

            this.Property(t => t.CorrectiveActionTaken)
                .HasMaxLength(250);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDFireHydrantandSprinklerSystem");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.BuildingName).HasColumnName("BuildingName");
            this.Property(t => t.DateOfInspection).HasColumnName("DateOfInspection");
            this.Property(t => t.CheckedBy).HasColumnName("CheckedBy");
            this.Property(t => t.Findings).HasColumnName("Findings");
            this.Property(t => t.RootCause).HasColumnName("RootCause");
            this.Property(t => t.CorrectiveActionTaken).HasColumnName("CorrectiveActionTaken");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
