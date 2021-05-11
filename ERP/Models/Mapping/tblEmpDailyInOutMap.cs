using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblEmpDailyInOutMap : EntityTypeConfiguration<tblEmpDailyInOut>
    {
        public tblEmpDailyInOutMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SrNo, t.EmployeeId });

            // Properties
            this.Property(t => t.SrNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Ecode)
                .HasMaxLength(10);

            this.Property(t => t.InComments)
                .HasMaxLength(255);

            this.Property(t => t.OutComments)
                .HasMaxLength(255);

            this.Property(t => t.ComputerName)
                .HasMaxLength(50);

            this.Property(t => t.MacAddress)
                .HasMaxLength(100);

            this.Property(t => t.IPAddress)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("tblEmpDailyInOut");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.Ecode).HasColumnName("Ecode");
            this.Property(t => t.Intime).HasColumnName("Intime");
            this.Property(t => t.OutTime).HasColumnName("OutTime");
            this.Property(t => t.InComments).HasColumnName("InComments");
            this.Property(t => t.OutComments).HasColumnName("OutComments");
            this.Property(t => t.InType).HasColumnName("InType");
            this.Property(t => t.OutType).HasColumnName("OutType");
            this.Property(t => t.ComputerName).HasColumnName("ComputerName");
            this.Property(t => t.MacAddress).HasColumnName("MacAddress");
            this.Property(t => t.IPAddress).HasColumnName("IPAddress");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChangedBy).HasColumnName("ChangedBy");

            // Relationships
            this.HasRequired(t => t.tblEmpPersonalInformation)
                .WithMany(t => t.tblEmpDailyInOuts)
                .HasForeignKey(d => d.EmployeeId);
            this.HasRequired(t => t.tblEmpPersonalInformation1)
                .WithMany(t => t.tblEmpDailyInOuts1)
                .HasForeignKey(d => d.EmployeeId);

        }
    }
}
