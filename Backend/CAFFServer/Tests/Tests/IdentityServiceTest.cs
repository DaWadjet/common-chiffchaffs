using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Base;

namespace Tests
{
    public class IdentityServiceTest : UnitTestBase
    {
        [Fact]
        public void Test_GetCurrentUserId()
        {
            var userId = identityServiceUser.GetCurrentUserId();

            Assert.Equal(UserId, userId);
        }

        [Fact]
        public async Task Test_GetCurrentUser()
        {
            var user = await identityServiceUser.GetCurrentUser();

            Assert.Equal(UserId, user.Id);
        }

        [Fact]
        public void Test_Admin_GetCurrentUserId()
        {
            var userId = identityServiceAdmin.GetCurrentUserId();

            Assert.Equal(AdminUserId, userId);
        }

        [Fact]
        public async Task Test_Admin_GetCurrentUser()
        {
            var user = await identityServiceAdmin.GetCurrentUser();

            Assert.Equal(AdminUserId, user.Id);
        }
    }
}
