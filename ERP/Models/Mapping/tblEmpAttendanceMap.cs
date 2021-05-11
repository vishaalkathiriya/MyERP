using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpAttendanceMap : EntityTypeConfiguration<tblEmpAttendance>
    {
        public tblEmpAttendanceMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Remark)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblEmpAttendance");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.PDate).HasColumnName("PDate");
            this.Property(t => t.Presence).HasColumnName("Presence");
            this.Property(t => t.Absence).HasColumnName("Absence");
            this.Property(t => t.Leave).HasColumnName("Leave");
            this.Property(t => t.OT).HasColumnName("OT");
            this.Property(t => t.WorkingHours).HasColumnName("WorkingHours");
            this.Property(t => t.PersonalWorkHours).HasColumnName("PersonalWorkHours");
            this.Property(t => t.CompanyWorkHours).HasColumnName("CompanyWorkHours");
            this.Property(t => t.LunchBreakHours).HasColumnName("LunchBreakHours");
            this.Property(t => t.IsHoliday).HasColumnName("IsHoliday");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
