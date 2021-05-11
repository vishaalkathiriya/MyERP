using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEMSGroupMap : EntityTypeConfiguration<tblEMSGroup>
    {
        public tblEMSGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.ClientGroupID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("tblEMSGroup");
            this.Property(t => t.ClientGroupID).HasColumnName("ClientGroupID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
        }
    }
}
