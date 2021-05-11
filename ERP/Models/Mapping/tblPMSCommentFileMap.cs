using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ERP.Models.Mapping
{
    public class tblPMSCommentFileMap : EntityTypeConfiguration<tblPMSCommentFile>
    {
        public tblPMSCommentFileMap()
        {
            // Primary Key
            this.HasKey(t => t.UploadedFileId);

            // Properties
            this.Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CaptionText)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblPMSCommentFile");
            this.Property(t => t.UploadedFileId).HasColumnName("UploadedFileId");
            this.Property(t => t.CommentId).HasColumnName("CommentId");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.CaptionText).HasColumnName("CaptionText");
            this.Property(t => t.CreDate).HasColumnName("CreDate");
            this.Property(t => t.CreBy).HasColumnName("CreBy");

            // Relationships
            this.HasRequired(t => t.tblPMSComment)
                .WithMany(t => t.tblPMSCommentFiles)
                .HasForeignKey(d => d.CommentId);

        }
    }
}
