using System;
using System.Linq;
using Radiant.Common.Tests;
using Radiant.Notifier.DataBase;
using Xunit;

namespace Radiant.Notifier.UnitTests.DataBase
{
    public class NotificationsDbContextTests
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void RemoveAllNotifications()
        {
            using NotificationsDbContext _DbContext = new();

            foreach (RadiantNotificationModel _Notification in _DbContext.Notifications)
                _DbContext.Notifications.Remove(_Notification);

            _DbContext.SaveChanges();
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void BasicNotificationDbContextTest()
        {
            using (NotificationsDbContext _DbContext = new())
            {
                RemoveAllNotifications();
                Assert.Equal(0, _DbContext.Notifications.Count());

                RadiantNotificationModel _NewNotification = new()
                {
                    Content = "UnitTest AE2F5562-B7A4-48D0-BAF0-813E146B9509",
                    Subject = "Radiant Notifier UnitTest",
                    EmailFrom = "Radiant UnitTesting unit",
                    EmailTo = { RadiantCommonUnitTestsConstants.EMAIL, RadiantCommonUnitTestsConstants.EMAIL },// We want to test the duplicate error handling as well
                    MinimalDateTimetoSend = DateTime.Now
                };

                _DbContext.Notifications.Add(_NewNotification);
                _DbContext.SaveChanges();

                Assert.Equal(1, _DbContext.Notifications.Count());
                Assert.False(_DbContext.Notifications.Single().Sent);
            }

            // Time to evaluate notifications
            RadiantNotificationsManager.EvaluateNotificationsInStorage();

            using (NotificationsDbContext _DbContext = new())
            {
                Assert.True(_DbContext.Notifications.Single().Sent);
            }

            // Clean up TODO
            RemoveAllNotifications();
        }
    }
}
