using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class VisitorMasterMap : EntityTypeConfiguration<VisitorMaster>
    {
        public VisitorMasterMap()
        {
            // Primary Key
            this.HasKey(t => new { t.VisitorId, t.MobileNo });

            // Properties
            this.Property(t => t.VisitorId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.MobileNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Address)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.Company)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Designation)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Remark)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("VisitorMaster");
            this.Property(t => t.VisitorId).HasColumnName("VisitorId");
            this.Property(t => t.MobileNo).HasColumnName("MobileNo");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Company).HasColumnName("Company");
            this.Property(t => t.Designation).HasColumnName("Designation");
            this.Property(t => t.PDate).HasColumnName("PDate");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Photo).HasColumnName("Photo");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
