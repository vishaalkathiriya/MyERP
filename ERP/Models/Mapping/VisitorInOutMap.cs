using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class VisitorInOutMap : EntityTypeConfiguration<VisitorInOut>
    {
        public VisitorInOutMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SrNo, t.VisitorId });

            // Properties
            this.Property(t => t.SrNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VisitorId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RefName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Department)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Manager)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Reason)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("VisitorInOut");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.VisitorId).HasColumnName("VisitorId");
            this.Property(t => t.ECode).HasColumnName("ECode");
            this.Property(t => t.RefName).HasColumnName("RefName");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.Manager).HasColumnName("Manager");
            this.Property(t => t.ExtNo).HasColumnName("ExtNo");
            this.Property(t => t.InTime).HasColumnName("InTime");
            this.Property(t => t.OutTime).HasColumnName("OutTime");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.Person).HasColumnName("Person");
        }
    }
}
