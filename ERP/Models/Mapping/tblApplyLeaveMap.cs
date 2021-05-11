using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblApplyLeaveMap : EntityTypeConfiguration<tblApplyLeave>
    {
        public tblApplyLeaveMap()
        {
            // Primary Key
            this.HasKey(t => t.LeaveId);

            // Properties
            this.Property(t => t.LeaveTitle)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PartFullTime)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.Comments)
                .HasMaxLength(255);

            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("tblApplyLeave");
            this.Property(t => t.LeaveId).HasColumnName("LeaveId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.LeaveTitle).HasColumnName("LeaveTitle");
            this.Property(t => t.LeaveType).HasColumnName("LeaveType");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.PartFullTime).HasColumnName("PartFullTime");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ApproveReason).HasColumnName("ApproveReason");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");

            // Relationships
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblApplyLeaves)
                .HasForeignKey(d => d.EmployeeId);

        }
    }
}
