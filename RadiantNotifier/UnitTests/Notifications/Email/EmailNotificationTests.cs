using System.Collections.Generic;
using System.Text;
using Radiant.Common.Tests;
using Radiant.Notifier.Notifications.Email;
using Radiant.Notifier.Notifications.Email.Mailkit;
using Xunit;

namespace Radiant.Notifier.UnitTests.Notifications.Email
{
    public class EmailNotificationTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void SendNotificationByEmail()
        {
            RadiantEmailNotification _EmailNotification = new RadiantEmailNotification();

            var _MailRequest = new MailRequest
            {
                Body = "UnitTest 9DDF162F-42C1-4D84-825C-D881CC983CC8",
                Subject = "Radiant-UnitTest",
                EmailFrom = "Radiant-UnitTest-SendNotificationByEmail",
                Attachments = new List<MailRequestAttachment>
                {
                    new()
                    {
                        FileName = "FileName_3fa2ced1-8880-417b-929c-a18f1f1b2cb5.txt",
                        Content = Encoding.ASCII.GetBytes("UNIT_TEST_56cfaad1-4d88-42bf-b71f-e0860eeef701"),
                        MediaType = "Text",
                        MediaSubType = ""
                    }
                }
            };
            _MailRequest.ToAddresses = new List<string> { RadiantCommonUnitTestsConstants.EMAIL };

            Assert.True(_EmailNotification.Send(_MailRequest));
        }
    }
}
