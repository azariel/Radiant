using RadiantNotifier.Notifications.Email;
using RadiantNotifier.Notifications.Email.Mailkit;
using Xunit;

namespace RadiantNotifier.UnitTests.Notifications.Email
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
            _EmailNotification.Send(new MailRequest
            {
                ToAddresses = { "frost.qc@gmail.com" },
                Body = "UnitTest 9DDF162F-42C1-4D84-825C-D881CC983CC8",
                Subject = "Radiant-UnitTest",
                FromAddresses = { "frost.qc2@gmail.com" }
            });
        }
    }
}
