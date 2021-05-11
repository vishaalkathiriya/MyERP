using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblDesignationMap : EntityTypeConfiguration<tblDesignation>
    {
        public tblDesignationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Designation)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblDesignation");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Designation).HasColumnName("Designation");
            this.Property(t => t.DesignationGroupId).HasColumnName("DesignationGroupId");
            this.Property(t => t.DesignationParentId).HasColumnName("DesignationParentId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblDesignationGroup)
                .WithMany(t => t.tblDesignations)
                .HasForeignKey(d => d.DesignationGroupId);
            this.HasRequired(t => t.tblDesignationParent)
                .WithMany(t => t.tblDesignations)
                .HasForeignKey(d => d.DesignationParentId);

        }
    }
}
