using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDFirstAIdLogBookMap : EntityTypeConfiguration<tblHRDFirstAIdLogBook>
    {
        public tblHRDFirstAIdLogBookMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.NameOfIssuer)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NameOfReceiver)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NameOfFirstAIdItems)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ManagerName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.LocationOfFirstAIdBox)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDFirstAIdLogBook");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.NameOfIssuer).HasColumnName("NameOfIssuer");
            this.Property(t => t.NameOfReceiver).HasColumnName("NameOfReceiver");
            this.Property(t => t.NameOfFirstAIdItems).HasColumnName("NameOfFirstAIdItems");
            this.Property(t => t.DateOfIssue).HasColumnName("DateOfIssue");
            this.Property(t => t.Quanity).HasColumnName("Quanity");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.ManagerName).HasColumnName("ManagerName");
            this.Property(t => t.LocationOfFirstAIdBox).HasColumnName("LocationOfFirstAIdBox");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
