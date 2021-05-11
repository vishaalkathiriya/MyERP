using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblINVConversationMap : EntityTypeConfiguration<tblINVConversation>
    {
        public tblINVConversationMap()
        {
            // Primary Key
            this.HasKey(t => t.PKConversationId);

            // Properties
            this.Property(t => t.ContentType)
                .IsRequired()
                .HasMaxLength(2);

            this.Property(t => t.ConversationTitle)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ConversationDescription)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tblINVConversation");
            this.Property(t => t.PKConversationId).HasColumnName("PKConversationId");
            this.Property(t => t.FKClientId).HasColumnName("FKClientId");
            this.Property(t => t.ContentType).HasColumnName("ContentType");
            this.Property(t => t.ConversationTitle).HasColumnName("ConversationTitle");
            this.Property(t => t.ConversationDescription).HasColumnName("ConversationDescription");
            this.Property(t => t.ConversationType).HasColumnName("ConversationType");
            this.Property(t => t.ConversationDate).HasColumnName("ConversationDate");
            this.Property(t => t.ReferenceId).HasColumnName("ReferenceId");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.CreBy).HasColumnName("CreBy");
            this.Property(t => t.ChgBy).HasColumnName("ChgBy");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.ChgDate).HasColumnName("ChgDate");

            // Relationships
            this.HasRequired(t => t.tblINVClient)
                .WithMany(t => t.tblINVConversations)
                .HasForeignKey(d => d.FKClientId);

        }
    }
}
