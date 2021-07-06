﻿using System;
using System.Linq;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Notifier.Configuration;
using Radiant.Notifier.DataBase;
using Radiant.Notifier.Notifications.Email;
using Radiant.Notifier.Notifications.Email.Mailkit;

namespace Radiant.Notifier
{
    public static class RadiantNotificationsManager
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static bool SendMailNotification(RadiantNotificationModel aRadiantNotificationModel)
        {
            MailRequest _MailRequest = new()
            {
                Subject = aRadiantNotificationModel.Subject,
                Body = aRadiantNotificationModel.Content,
                ToAddresses = aRadiantNotificationModel.EmailTo.Distinct().ToList(),
                EmailFrom = aRadiantNotificationModel.EmailFrom
            };

            return new RadiantEmailNotification().Send(_MailRequest);
        }

        private static bool SendNotification(RadiantNotificationModel aRadiantNotificationModel)
        {
            if (aRadiantNotificationModel == null)
                return false;

            switch (aRadiantNotificationModel.NotificationType)
            {
                case RadiantNotificationModel.RadiantNotificationType.Email:
                    return SendMailNotification(aRadiantNotificationModel);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void HandleNotificationFailure(RadiantNotificationModel aRadiantNotificationModel)
        {
            if (aRadiantNotificationModel == null)
                return;

            LoggingManager.LogToFile("0FBFDBD8-DE63-4CBA-9561-C4C52560A189", $"Failed to send notification ID [{aRadiantNotificationModel.NotificationId}]. This notification will be re-queued.");
        }

        private static void HandleNotificationSuccess(RadiantNotificationModel aRadiantNotificationModel)
        {
            if (aRadiantNotificationModel == null)
                return;

            aRadiantNotificationModel.Sent = true;

            RadiantNotificationConfig _Configuration = NotificationConfigurationManager.ReloadConfig();
            if (_Configuration.EmailServer.SleepMsBetweenEmailSent > 0)
                Thread.Sleep(_Configuration.EmailServer.SleepMsBetweenEmailSent);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        /// <summary>
        /// Evaluate notifications in storage and send those that are ready to be sent.
        /// </summary>
        public static void EvaluateNotificationsInStorage()
        {
            using NotificationsDbContext _DbContext = new();

            var _Now = DateTime.Now;
            foreach (RadiantNotificationModel _NotificationToSend in _DbContext.Notifications.Where(w => !w.Sent && w.MinimalDateTimetoSend < _Now))
            {
                bool _Result = SendNotification(_NotificationToSend);

                if (!_Result)
                    HandleNotificationFailure(_NotificationToSend);
                else
                    HandleNotificationSuccess(_NotificationToSend);

                _DbContext.SaveChanges();
            }
        }
    }
}
