using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class teledataMap : EntityTypeConfiguration<teledata>
    {
        public teledataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SrNo, t.Type, t.PDate, t.Outline, t.Duration, t.Ext, t.Ringing });

            // Properties
            this.Property(t => t.SrNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Outline)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Duration)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Ext)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.OutNumber)
                .HasMaxLength(100);

            this.Property(t => t.Ringing)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("teledata");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.PDate).HasColumnName("PDate");
            this.Property(t => t.Outline).HasColumnName("Outline");
            this.Property(t => t.Duration).HasColumnName("Duration");
            this.Property(t => t.Ext).HasColumnName("Ext");
            this.Property(t => t.OutNumber).HasColumnName("OutNumber");
            this.Property(t => t.Ringing).HasColumnName("Ringing");
        }
    }
}
