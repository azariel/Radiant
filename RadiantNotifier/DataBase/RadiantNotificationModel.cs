﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Notifier.DataBase
{
    [Table("Notifications")]
    public class RadiantNotificationModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Nested Types
        // ********************************************************************
        public enum RadiantNotificationType
        {
            Email
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [Required]
        [MaxLength(4096)]
        public string Content { get; set; } = "";

        [Required]
        [MaxLength(255)]
        public string EmailFrom { get; set; } = "Radiant Notifier Module";

        [Required]
        [MaxLength(2048)]
        public List<string> EmailTo { get; set; } = new();

        [Required]
        public DateTime MinimalDateTimetoSend { get; set; }

        [Required]
        [Key]
        public long NotificationId { get; set; }

        [Required]
        public RadiantNotificationType NotificationType { get; set; }

        public bool Sent { get; set; } = false;

        [Required]
        [MaxLength(255)]
        public string Subject { get; set; } = "";
    }
}