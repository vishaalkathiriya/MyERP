using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblABGroupMap : EntityTypeConfiguration<tblABGroup>
    {
        public tblABGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.GroupId);

            // Properties
            this.Property(t => t.GroupName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblABGroup");
            this.Property(t => t.GroupId).HasColumnName("GroupId");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
            this.Property(t => t.GroupNote).HasColumnName("GroupNote");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
