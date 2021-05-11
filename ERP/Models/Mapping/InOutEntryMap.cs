using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class InOutEntryMap : EntityTypeConfiguration<InOutEntry>
    {
        public InOutEntryMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ECode, t.SrNo });

            // Properties
            this.Property(t => t.ECode)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SrNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Status)
                .HasMaxLength(10);

            this.Property(t => t.Remark)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("InOutEntry");
            this.Property(t => t.ECode).HasColumnName("ECode");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.Machine).HasColumnName("Machine");
            this.Property(t => t.InOutTime).HasColumnName("InOutTime");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Remark).HasColumnName("Remark");
        }
    }
}
