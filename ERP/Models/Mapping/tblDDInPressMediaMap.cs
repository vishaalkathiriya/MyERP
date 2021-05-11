using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblDDInPressMediaMap : EntityTypeConfiguration<tblDDInPressMedia>
    {
        public tblDDInPressMediaMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.SrNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.NameOfNewspaper)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.EventName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Website)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Attachment)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CreBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ChgBy)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblDDInPressMedia");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.NameOfNewspaper).HasColumnName("NameOfNewspaper");
            this.Property(t => t.EventName).HasColumnName("EventName");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
