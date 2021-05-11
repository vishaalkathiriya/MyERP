using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVClientPersonMap : EntityTypeConfiguration<tblINVClientPerson>
    {
        public tblINVClientPersonMap()
        {
            // Primary Key
            this.HasKey(t => t.PKId);

            // Properties
            this.Property(t => t.Prefix)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.FullName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Designation)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IdentityNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MobileNo)
                .IsRequired()
                .HasMaxLength(13);

            // Table & Column Mappings
            this.ToTable("tblINVClientPerson");
            this.Property(t => t.PKId).HasColumnName("PKId");
            this.Property(t => t.FKClientId).HasColumnName("FKClientId");
            this.Property(t => t.Prefix).HasColumnName("Prefix");
            this.Property(t => t.FullName).HasColumnName("FullName");
            this.Property(t => t.Designation).HasColumnName("Designation");
            this.Property(t => t.IdentityDocId).HasColumnName("IdentityDocId");
            this.Property(t => t.IdentityNo).HasColumnName("IdentityNo");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.MobileNo).HasColumnName("MobileNo");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblINVClient)
                .WithMany(t => t.tblINVClientPersons)
                .HasForeignKey(d => d.FKClientId);

        }
    }
}
