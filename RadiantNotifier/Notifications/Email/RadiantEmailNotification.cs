using System;
using System.Linq;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Radiant.Common.Diagnostics;
using Radiant.Notifier.Configuration;
using Radiant.Notifier.Notifications.Email.Mailkit;

namespace Radiant.Notifier.Notifications.Email
{
    public class RadiantEmailNotification : IRadiantNotification
    {
        public bool Send(INotificationRequest aMailRequest)
        {
            // Documentation: https://github.com/jstedfast/MailKit/blob/master/GMailOAuth2.md

            if (aMailRequest is not MailRequest _MailRequest)
            {
                LoggingManager.LogToFile("F89E03F9-2733-43C1-B55D-BEDC80F7290F", $"{nameof(INotificationRequest)} is not of type {nameof(MailRequest)}. Will not send.");
                return false;
            }

            try
            {
                RadiantNotificationConfig _Configuration = NotificationConfigurationManager.ReloadConfig();

                var _HtmlBodyBuilder = new BodyBuilder
                {
                    HtmlBody = _MailRequest.Body
                };

                MimeMessage _Message = new()
                {
                    Subject = _MailRequest.Subject,
                    Body = _HtmlBodyBuilder.ToMessageBody()
                };
                _Message.From.Add(new MailboxAddress(_MailRequest.EmailFrom, _Configuration.EmailServer.SmtpUsername));
                _Message.To.AddRange(_MailRequest.ToAddresses.Select(s => new MailboxAddress(s, s)));

                var _Client = new SmtpClient();
                _Client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                _Client.Connect(_Configuration.EmailServer.SmtpServer, _Configuration.EmailServer.SmtpPort, SecureSocketOptions.StartTls);
                try
                {
                    _Client.Authenticate(_Configuration.EmailServer.SmtpUsername, _Configuration.EmailServer.SmtpPassword);
                    _Client.Send(_Message);
                } finally
                {
                    _Client.Disconnect(true);
                }

                return true;
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("36A9E2F2-5650-4689-ABB2-58B905FFB7E3", $"Couldn't send email to [{string.Join(",", _MailRequest.ToAddresses)}].", _Ex);
                return false;
            }
        }
    }
}
