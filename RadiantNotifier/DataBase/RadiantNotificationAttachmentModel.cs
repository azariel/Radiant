using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Notifier.DataBase
{
    [Table("NotificationAttachments")]
    public class RadiantNotificationAttachmentModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [Required]
        public byte[] Content { get; set; }

        [Required]
        [MaxLength(length: 512)]
        public string FileName { get; set; }

        [Required]
        [NotNull]
        [MaxLength(length: 512)]
        public string MediaSubType { get; set; }

        [Required]
        [NotNull]
        [MaxLength(length: 512)]
        public string MediaType { get; set; }

        [ForeignKey("NotificationId")]
        public virtual RadiantNotificationModel Notification { get; set; }

        [Required]
        [Key]
        public long NotificationAttachmentId { get; set; }

        public long NotificationId { get; set; }
    }
}
