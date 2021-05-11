using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDPressMediaExpensMap : EntityTypeConfiguration<tblHRDPressMediaExpens>
    {
        public tblHRDPressMediaExpensMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.NameOfPressMedia)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.RepresentativeName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.MobileNumber)
                .HasMaxLength(15);

            this.Property(t => t.ApprovedBy)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Occasion)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDPressMediaExpenses");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.NameOfPressMedia).HasColumnName("NameOfPressMedia");
            this.Property(t => t.RepresentativeName).HasColumnName("RepresentativeName");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.MobileNumber).HasColumnName("MobileNumber");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.ApprovedBy).HasColumnName("ApprovedBy");
            this.Property(t => t.Occasion).HasColumnName("Occasion");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
