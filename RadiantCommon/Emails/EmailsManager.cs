using System;
using System.Collections.Generic;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Radiant.Common.Emails.Mailkit;

namespace Radiant.Common.Emails
{
    /// <summary>
    /// Wrap operations around emails
    /// </summary>
    public class EmailsManager
    {
        private readonly EmailServerConfiguration fEmailServerConfiguration;

        public EmailsManager(EmailServerConfiguration emailServerConfiguration)
        {
            fEmailServerConfiguration = emailServerConfiguration;
        }

        public MimeMessage[] ParseMailbox(string containsSubject = null)
        {
            List<MimeMessage> messages = new();

            using (var client = new ImapClient())
            {
                client.Connect(fEmailServerConfiguration.SmtpServer, fEmailServerConfiguration.SmtpPort, true);
                client.Authenticate(fEmailServerConfiguration.SmtpUsername, fEmailServerConfiguration.SmtpPassword);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);


                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);

                    if (containsSubject == null || message.Subject.Contains(containsSubject, StringComparison.InvariantCultureIgnoreCase))
                        messages.Add(inbox.GetMessage(i));
                }

                client.Disconnect(true);
            }

            return messages.ToArray();
        }

        public bool DeleteMessageFromInbox(string messageId)
        {
            using (var client = new ImapClient())
            {
                client.Connect(fEmailServerConfiguration.SmtpServer, fEmailServerConfiguration.SmtpPort, true);
                client.Authenticate(fEmailServerConfiguration.SmtpUsername, fEmailServerConfiguration.SmtpPassword);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                try
                {
                    for (int i = 0; i < inbox.Count; i++)
                    {
                        if (!inbox.GetMessage(i).MessageId.Equals(messageId, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        inbox.AddFlags(i, MessageFlags.Deleted, null, false);
                        inbox.Expunge();// TODO: async ?
                    }
                }
                finally
                {
                    client.Disconnect(true);
                }

                return false;
            }
        }
    }
}

//var _Client = new SmtpClient();
//_Client.ServerCertificateValidationCallback = (s, c, h, e) => true;
//_Client.Connect(fEmailServerConfiguration.SmtpServer, fEmailServerConfiguration.SmtpPort, SecureSocketOptions.StartTls);
//try
//{
//    _Client.Authenticate(fEmailServerConfiguration.SmtpUsername, fEmailServerConfiguration.SmtpPassword);
//}
//finally
//{
//    _Client.Disconnect(true);
//}