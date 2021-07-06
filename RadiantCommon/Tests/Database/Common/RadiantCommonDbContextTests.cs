using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Radiant.Common.Database.Common;
using Xunit;

namespace Radiant.Common.Tests.Database.Common
{
    public class RadiantCommonDbContextTests
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void RemoveAllUsers()
        {
            using RadiantCommonDbContext _DbContext = new();

            foreach (RadiantUserModel _User in _DbContext.Users)
                _DbContext.Users.Remove(_User);

            _DbContext.SaveChanges();
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void BasicUserTest()
        {
            using var _DbContext = new RadiantCommonDbContext();
            RemoveAllUsers();
            Assert.Equal(0, _DbContext.Users.Count());

            RadiantUserModel _NewUser = new()
            {
                Email = RadiantCommonUnitTestsConstants.EMAIL,
                UserName = "testF0B7A213-76BD-4CAE-BA32-28A4786B8A19",
                Password = "test66F1778B-38DF-4043-8799-E384487FEF14",
                Type = RadiantUserModel.UserType.Admin
            };

            _DbContext.Users.Add(_NewUser);
            _DbContext.SaveChanges();

            Assert.Equal(1, _DbContext.Users.Count());

            // Clean
            RemoveAllUsers();
        }

        [Fact(Skip = "Add sa user")]
        //[Fact]
        public void AddSAUserPersistent()
        {
            using var _DataBaseContext = new RadiantCommonDbContext();

            RadiantUserModel _NewUser = new()
            {
                Email = RadiantCommonUnitTestsConstants.EMAIL,
                UserName = "sa",
                Password = "cuser",
                Type = RadiantUserModel.UserType.Admin
            };

            _DataBaseContext.Users.Add(_NewUser);
            _DataBaseContext.SaveChanges();
        }
    }
}
