using System;
using System.Collections.Generic;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
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

        public MimeMessage[] ParseMailbox(string containsSubject = null, string[] containsAttachmentsContainingString = null, string[] containsAttachmentsOfMediaTypes = null)
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
                    {
                        if (containsAttachmentsOfMediaTypes == null || containsAttachmentsOfMediaTypes.Length <= 0 || message.Attachments.Any(a => containsAttachmentsOfMediaTypes.Any(b => b.Equals(a.ContentType.MediaSubtype, StringComparison.InvariantCultureIgnoreCase))))
                        {
                            if (containsAttachmentsContainingString == null || containsAttachmentsContainingString.Length <= 0 ||
                                message.Attachments.OfType<MimePart>().Any(a => containsAttachmentsContainingString.Any(b => a.FileName.Contains(b, StringComparison.InvariantCultureIgnoreCase))))
                            {
                                messages.Add(inbox.GetMessage(i));
                            }
                        }
                    }
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
                        return true;
                    }
                } finally
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