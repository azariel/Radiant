using System;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
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
        private static void HandleNotificationFailure(RadiantNotificationModel aRadiantNotificationModel)
        {
            if (aRadiantNotificationModel == null)
                return;

            // Timeout notification for an hour TODO: make this configurable ?
            aRadiantNotificationModel.MinimalDateTimetoSend = DateTime.Now.AddHours(value: 1);

            LoggingManager.LogToFile("0FBFDBD8-DE63-4CBA-9561-C4C52560A189", $"Failed to send notification ID [{aRadiantNotificationModel.NotificationId}]. This notification will be re-queued.");
        }

        private static void HandleNotificationSuccess(RadiantNotificationModel aRadiantNotificationModel)
        {
            if (aRadiantNotificationModel == null)
                return;

            aRadiantNotificationModel.Sent = true;
        }

        private static bool SendMailNotification(RadiantNotificationModel aRadiantNotificationModel)
        {
            MailRequest _MailRequest = new()
            {
                Subject = aRadiantNotificationModel.Subject,
                Body = aRadiantNotificationModel.Content,
                ToAddresses = aRadiantNotificationModel.EmailTo.Distinct().ToList(),
                EmailFrom = aRadiantNotificationModel.EmailFrom,
                Attachments = aRadiantNotificationModel.Attachments.Select(s => new MailRequestAttachment
                {
                    FileName = s.FileName,
                    Content = s.Content,
                    MediaType = s.MediaType,
                    MediaSubType = s.MediaSubType
                }).ToList()
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
                case RadiantNotificationModel.RadiantNotificationType.SystemTrayPopup:
                    return SendSystemTrayPopupNotification(aRadiantNotificationModel);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool SendSystemTrayPopupNotification(RadiantNotificationModel aRadiantNotificationModel)
        {
            throw new NotImplementedException("System tray popup notifications are unhandled at this moment.");
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
            _DbContext.Notifications.Load();
            _DbContext.NotificationAttachments.Load();

            var _Now = DateTime.Now;
            foreach (RadiantNotificationModel _NotificationToSend in _DbContext.Notifications.Where(w => !w.Sent && w.MinimalDateTimetoSend < _Now))
            {
                bool _Result = SendNotification(_NotificationToSend);

                if (!_Result)
                    HandleNotificationFailure(_NotificationToSend);
                else
                    HandleNotificationSuccess(_NotificationToSend);

                RadiantNotificationConfig _Configuration = NotificationConfigurationManager.ReloadConfig();
                if (_Configuration.EmailServer.SleepMsBetweenEmailSent > 0)
                    Thread.Sleep(_Configuration.EmailServer.SleepMsBetweenEmailSent);

                int _RetryCounter = 0;
                while (true)
                {
                    if (_RetryCounter > 300)// 30 sec
                    {
                        LoggingManager.LogToFile("8DFC5A7A-EB6F-4726-8C68-A59BDC753C82", $"Error. Couldn't add notification to database. Subject: {_NotificationToSend.Subject} Destination: {string.Join(",", _NotificationToSend.EmailTo)}.");
                        return;
                    }

                    try
                    {
                        _DbContext.SaveChanges();
                        return;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(millisecondsTimeout: 100);
                    }

                    ++_RetryCounter;
                }
            }
        }
    }
}
