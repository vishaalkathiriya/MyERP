using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblPMSCommentMap : EntityTypeConfiguration<tblPMSComment>
    {
        public tblPMSCommentMap()
        {
            // Primary Key
            this.HasKey(t => t.CommentId);

            // Properties
            this.Property(t => t.CommentText)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("tblPMSComment");
            this.Property(t => t.CommentId).HasColumnName("CommentId");
            this.Property(t => t.TodoId).HasColumnName("TodoId");
            this.Property(t => t.CommentText).HasColumnName("CommentText");
            this.Property(t => t.Hours).HasColumnName("Hours");
            this.Property(t => t.IsArchived).HasColumnName("IsArchived");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");

            // Relationships
            this.HasRequired(t => t.tblPMSToDo)
                .WithMany(t => t.tblPMSComments)
                .HasForeignKey(d => d.TodoId);

        }
    }
}
