using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblPMSProjectUserMap : EntityTypeConfiguration<tblPMSProjectUser>
    {
        public tblPMSProjectUserMap()
        {
            // Primary Key
            this.HasKey(t => t.ProjectUserId);

            // Properties
            // Table & Column Mappings
            this.ToTable("tblPMSProjectUser");
            this.Property(t => t.ProjectUserId).HasColumnName("ProjectUserId");
            this.Property(t => t.ProjectId).HasColumnName("ProjectId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.IsTL).HasColumnName("IsTL");
            this.Property(t => t.UserUnder).HasColumnName("UserUnder");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblPMSProjectUsers)
                .HasForeignKey(d => d.EmployeeId);
            this.HasRequired(t => t.tblPMSProject)
                .WithMany(t => t.tblPMSProjectUsers)
                .HasForeignKey(d => d.ProjectId);

        }
    }
}
