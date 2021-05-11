using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpRelativeRelationMap : EntityTypeConfiguration<tblEmpRelativeRelation>
    {
        public tblEmpRelativeRelationMap()
        {
            // Primary Key
            this.HasKey(t => t.RelationId);

            // Properties
            this.Property(t => t.RelativeRelationName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblEmpRelativeRelation");
            this.Property(t => t.RelationId).HasColumnName("RelationId");
            this.Property(t => t.RelativeRelationName).HasColumnName("RelativeRelationName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
        }
    }
}
