using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDBoilPlantCleaningRecordMap : EntityTypeConfiguration<tblHRDBoilPlantCleaningRecord>
    {
        public tblHRDBoilPlantCleaningRecordMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.BoilPlantLocation)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NameOfCleaner)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.PlantIncharge)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDBoilPlantCleaningRecords");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.BoilPlantLocation).HasColumnName("BoilPlantLocation");
            this.Property(t => t.DateOfCleaining).HasColumnName("DateOfCleaining");
            this.Property(t => t.NameOfCleaner).HasColumnName("NameOfCleaner");
            this.Property(t => t.PlantIncharge).HasColumnName("PlantIncharge");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
