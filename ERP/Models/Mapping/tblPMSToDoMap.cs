using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblPMSToDoMap : EntityTypeConfiguration<tblPMSToDo>
    {
        public tblPMSToDoMap()
        {
            // Primary Key
            this.HasKey(t => t.TodoId);

            // Properties
            this.Property(t => t.TodoText)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("tblPMSToDo");
            this.Property(t => t.TodoId).HasColumnName("TodoId");
            this.Property(t => t.ModuleId).HasColumnName("ModuleId");
            this.Property(t => t.TodoText).HasColumnName("TodoText");
            this.Property(t => t.AssignedUser).HasColumnName("AssignedUser");
            this.Property(t => t.AssignedHours).HasColumnName("AssignedHours");
            this.Property(t => t.TodoType).HasColumnName("TodoType");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.DueDate).HasColumnName("DueDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Priority).HasColumnName("Priority");
            this.Property(t => t.IsArchived).HasColumnName("IsArchived");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CanFinish).HasColumnName("CanFinish");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
