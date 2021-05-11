using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblDesignationGroupMap : EntityTypeConfiguration<tblDesignationGroup>
    {
        public tblDesignationGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.DesignationGroup)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblDesignationGroup");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DesignationGroup).HasColumnName("DesignationGroup");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
