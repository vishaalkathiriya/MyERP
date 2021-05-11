using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblHRDFireExtinguiserLogBookMap : EntityTypeConfiguration<tblHRDFireExtinguiserLogBook>
    {
        public tblHRDFireExtinguiserLogBookMap()
        {
            // Primary Key
            this.HasKey(t => t.SrNo);

            // Properties
            this.Property(t => t.TypeOfFireExtinguiser)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Location)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.UsedOfFireExtinguiser)
                .HasMaxLength(100);

            this.Property(t => t.Reason)
                .HasMaxLength(100);

            this.Property(t => t.Remark)
                .HasMaxLength(250);

            this.Property(t => t.Attachment)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("tblHRDFireExtinguiserLogBook");
            this.Property(t => t.SrNo).HasColumnName("SrNo");
            this.Property(t => t.TypeOfFireExtinguiser).HasColumnName("TypeOfFireExtinguiser");
            this.Property(t => t.Capacity).HasColumnName("Capacity");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.DateOfInspection).HasColumnName("DateOfInspection");
            this.Property(t => t.UsedOfFireExtinguiser).HasColumnName("UsedOfFireExtinguiser");
            this.Property(t => t.DateOfRefilling).HasColumnName("DateOfRefilling");
            this.Property(t => t.DueDateForNextRefilling).HasColumnName("DueDateForNextRefilling");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
        }
    }
}
