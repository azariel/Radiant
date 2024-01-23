using MimeKit;
using Radiant.Common.Diagnostics;
using Radiant.Common.Emails;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Configuration;

namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Tasks
{
    public class FetchAttachmentsFromEmailTask : RadiantTask
    {
        protected override void TriggerNowImplementation()
        {
            // Fetch pending emails from SMTP
            var _Config = RadiantBudgetBridgeConfigurationManager.ReloadConfig();
            EmailsManager _EmailsManager = new EmailsManager(_Config.ImapConfiguration);

            MimeMessage[] _Emails = _EmailsManager.ParseMailbox("budget");

            foreach (var _Email in _Emails)
            {
                if (!_Email.Attachments.Any())
                {
                    LoggingManager.LogToFile("6e6888d3-bce6-450c-815d-ef6aac3b4817", $"Email [{_Email.Subject}] didn't contains an attachment. Email will be deleted.");
                    _EmailsManager.DeleteMessageFromInbox(_Email.MessageId);
                    continue;
                }

                if (!_Email.From.Mailboxes.Any())
                {
                    LoggingManager.LogToFile("485d7402-260c-4452-b2e4-fe131048a0c0", $"Email [{_Email.Subject}] sender couldn't be found. No FROM mailboxes. Email will be deleted.");
                    _EmailsManager.DeleteMessageFromInbox(_Email.MessageId);
                    continue;
                }

                // Download attachment to current Dir
                foreach (MimeEntity _EmailAttachment in _Email.Attachments)
                {
                    using (var stream = File.Create($"HistoryTransactions_{_Email.From.Mailboxes.First().Address.Replace("@", ".")}.csv"))
                    {
                        if (_EmailAttachment is MessagePart)
                        {
                            var part = (MessagePart)_EmailAttachment;

                            part.Message.WriteTo(stream);
                        }
                        else
                        {
                            var part = (MimePart)_EmailAttachment;

                            part.Content.DecodeTo(stream);
                        }
                    }
                }
            }
        }
    }
}
