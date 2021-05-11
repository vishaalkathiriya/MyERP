using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblCMCustomerDetailMap : EntityTypeConfiguration<tblCMCustomerDetail>
    {
        public tblCMCustomerDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.CustomerDetailId);

            // Properties
            this.Property(t => t.Title)
                .HasMaxLength(50);

            this.Property(t => t.Value)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblCMCustomerDetail");
            this.Property(t => t.CustomerDetailId).HasColumnName("CustomerDetailId");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblCMCustomer)
                .WithMany(t => t.tblCMCustomerDetails)
                .HasForeignKey(d => d.CustomerId);

        }
    }
}
