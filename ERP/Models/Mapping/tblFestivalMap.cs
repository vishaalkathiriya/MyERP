using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblFestivalMap : EntityTypeConfiguration<tblFestival>
    {
        public tblFestivalMap()
        {
            // Primary Key
            this.HasKey(t => t.FestivalId);

            // Properties
            this.Property(t => t.FestivalName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblFestivals");
            this.Property(t => t.FestivalId).HasColumnName("FestivalId");
            this.Property(t => t.FestivalName).HasColumnName("FestivalName");
            this.Property(t => t.FestivalDate).HasColumnName("FestivalDate");
            this.Property(t => t.FestivalTypeId).HasColumnName("FestivalTypeId");
            this.Property(t => t.FestivalGroupId).HasColumnName("FestivalGroupId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblFestivalType)
                .WithMany(t => t.tblFestivals)
                .HasForeignKey(d => d.FestivalTypeId);

        }
    }
}
