using System.Collections.Generic;
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
                EmailFrom = "Radiant-UnitTest-SendNotificationByEmail"
            };
            _MailRequest.ToAddresses = new List<string> { RadiantCommonUnitTestsConstants.EMAIL };

            _EmailNotification.Send(_MailRequest);
        }
    }
}
