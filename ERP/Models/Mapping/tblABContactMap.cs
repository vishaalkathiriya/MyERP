using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblABContactMap : EntityTypeConfiguration<tblABContact>
    {
        public tblABContactMap()
        {
            // Primary Key
            this.HasKey(t => t.ContactId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.PhoneNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LandlineNo)
                .HasMaxLength(50);

            this.Property(t => t.Address1)
                .HasMaxLength(255);

            this.Property(t => t.Address2)
                .HasMaxLength(255);

            this.Property(t => t.Area)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.City)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Pincode)
                .HasMaxLength(50);

            this.Property(t => t.CompanyName)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblABContact");
            this.Property(t => t.ContactId).HasColumnName("ContactId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PhoneNo).HasColumnName("PhoneNo");
            this.Property(t => t.LandlineNo).HasColumnName("LandlineNo");
            this.Property(t => t.Address1).HasColumnName("Address1");
            this.Property(t => t.Address2).HasColumnName("Address2");
            this.Property(t => t.Area).HasColumnName("Area");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.Pincode).HasColumnName("Pincode");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.LangId).HasColumnName("LangId");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblABLanguage)
                .WithMany(t => t.tblABContacts)
                .HasForeignKey(d => d.LangId);

        }
    }
}
