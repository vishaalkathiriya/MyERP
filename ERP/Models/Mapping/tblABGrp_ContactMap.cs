using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblABGrp_ContactMap : EntityTypeConfiguration<tblABGrp_Contact>
    {
        public tblABGrp_ContactMap()
        {
            // Primary Key
            this.HasKey(t => t.Grp_ContactId);

            // Properties
            // Table & Column Mappings
            this.ToTable("tblABGrp_Contact");
            this.Property(t => t.Grp_ContactId).HasColumnName("Grp_ContactId");
            this.Property(t => t.ContactId).HasColumnName("ContactId");
            this.Property(t => t.GroupId).HasColumnName("GroupId");

            // Relationships
            this.HasRequired(t => t.tblABContact)
                .WithMany(t => t.tblABGrp_Contact)
                .HasForeignKey(d => d.ContactId);
            this.HasRequired(t => t.tblABGroup)
                .WithMany(t => t.tblABGrp_Contact)
                .HasForeignKey(d => d.GroupId);

        }
    }
}
