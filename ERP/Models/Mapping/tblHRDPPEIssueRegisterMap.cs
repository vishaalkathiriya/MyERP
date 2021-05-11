using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDPPEIssueRegisterMap : EntityTypeConfiguration<tblHRDPPEIssueRegister>
    {
        public tblHRDPPEIssueRegisterMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.NameOfIssuer)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NameOfRecievr)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.TypeOfPPE)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Department)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ManagerName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDPPEIssueRegister");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.NameOfIssuer).HasColumnName("NameOfIssuer");
            this.Property(t => t.NameOfRecievr).HasColumnName("NameOfRecievr");
            this.Property(t => t.TypeOfPPE).HasColumnName("TypeOfPPE");
            this.Property(t => t.Quanity).HasColumnName("Quanity");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.ManagerName).HasColumnName("ManagerName");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
