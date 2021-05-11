using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblFestivalTypeMap : EntityTypeConfiguration<tblFestivalType>
    {
        public tblFestivalTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.FestivalTypeId);

            // Properties
            this.Property(t => t.FestivalType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PartFullTime)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.DisplayColorCode)
                .IsRequired()
                .HasMaxLength(7);

            // Table & Column Mappings
            this.ToTable("tblFestivalTypes");
            this.Property(t => t.FestivalTypeId).HasColumnName("FestivalTypeId");
            this.Property(t => t.FestivalType).HasColumnName("FestivalType");
            this.Property(t => t.IsWorkingDay).HasColumnName("IsWorkingDay");
            this.Property(t => t.PartFullTime).HasColumnName("PartFullTime");
            this.Property(t => t.DisplayColorCode).HasColumnName("DisplayColorCode");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
