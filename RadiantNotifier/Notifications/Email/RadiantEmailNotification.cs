using System;
using System.Linq;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Radiant.Common.Diagnostics;
using RadiantNotifier.Configuration;
using RadiantNotifier.Notifications.Email.Mailkit;

namespace RadiantNotifier.Notifications.Email
{
    public class RadiantEmailNotification : IRadiantNotification
    {
        public bool Send(INotificationRequest aMailRequest)
        {
            if (aMailRequest is not MailRequest _MailRequest)
            {
                LoggingManager.LogToFile($"{nameof(INotificationRequest)} is not of type {nameof(MailRequest)}. Will not send.");
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
                    Body = _HtmlBodyBuilder.ToMessageBody(),
                    From = { new MailboxAddress(_Configuration.EmailServer.EmailFrom, _Configuration.EmailServer.EmailFrom) }
                };
                _Message.To.AddRange(_MailRequest.ToAddresses.Select(s => new MailboxAddress(s, s)));

                var _Client = new SmtpClient();
                _Client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                _Client.Connect(_Configuration.EmailServer.SmtpServer, _Configuration.EmailServer.SmtpPort, SecureSocketOptions.SslOnConnect);
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
                LoggingManager.LogToFile($"Couldn't send email to [{string.Join(",", _MailRequest.ToAddresses)}].", _Ex);
                return false;
            }
        }
    }
}
