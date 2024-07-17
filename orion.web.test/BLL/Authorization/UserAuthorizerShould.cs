/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Orion.Web.BLL.Authorization;
using Orion.Web.Common;
using Orion.Web.Employees;

namespace Orion.Web.test.BLL.Authorization
{
    [TestClass]
    public class UserAuthorizerShould
    {
        private class TestContext
        {
            private readonly ClaimsPrincipal _user;
            public readonly TestUser CurrentUser;
            private readonly Mock<IDirectReportsRepository> _directReportsRepo = new Mock<IDirectReportsRepository>();
            private readonly Mock<ISessionAdapter> _sessionAdapter = new Mock<ISessionAdapter>();
            public static HashSet<TestUser> TestUsers = new HashSet<TestUser>()
            {
                new TestUser( 1, UserRoleName.Admin ),
                new TestUser( 2, UserRoleName.Disabled ),
                new TestUser( 3, UserRoleName.Limited ),
                new TestUser( 4, UserRoleName.Standard ),
                new TestUser( 5, UserRoleName.Standard ),
                new TestUser( 6, UserRoleName.Standard ),
                new TestUser( 7, UserRoleName.Standard ),
                new TestUser( 8, UserRoleName.Supervisor, new[] {3,4,5} ),
                new TestUser( 9, UserRoleName.Supervisor, new[] {6,7} ),
            };
            public record TestUser(int UserId, string RoleName, params int[] DirectReports);
            public TestContext(Func<IEnumerable<TestUser>, TestUser> currentUserSelector)
            {
                CurrentUser = currentUserSelector(TestUsers);
                var ident = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Role, CurrentUser.RoleName)
                    }
                );
                _user = new ClaimsPrincipal(ident);

                _directReportsRepo.Setup(x => x.GetDirectReports(CurrentUser.UserId))
                                  .Returns(Task.FromResult(CurrentUser.DirectReports));

                _sessionAdapter.Setup(x => x.EmployeeIdAsync())
                               .Returns(Task.FromResult(CurrentUser.UserId));
            }

            public CurrentUserPermissionCheck GetItemUnderTest()
            {
                return new CurrentUserPermissionCheck(_user, _directReportsRepo.Object, _sessionAdapter.Object);
            }
        }

        [TestMethod]
        public void Only_Allow_Admins_To_Administer_Users()
        {
            foreach (var roleField in typeof(UserRoleName).GetFields())
            {
                var roleKey = roleField.GetValue(default) as string;
                var ctx = new TestContext(x => x.First(x => x.RoleName == roleKey));
                CurrentUserPermissionCheck underTest = ctx.GetItemUnderTest();
                var actual = underTest.CanAdministerUsers();
                if (roleKey == UserRoleName.Admin)
                {
                    actual.Should().BeTrue($"test role was {roleKey} and expected was {UserRoleName.Admin}");
                }
                else
                {
                    actual.Should().BeFalse($"test role was {roleKey} and expected was {UserRoleName.Admin}");
                }
            }
        }

        [TestMethod]
        public async Task When_Current_User_Is_Admin_And_Editing_Time_Then_Allowed()
        {
            var ctx = new TestContext(x => x.First(x => x.RoleName == UserRoleName.Admin));
            var underTest = ctx.GetItemUnderTest();
            foreach (var testUser in TestContext.TestUsers)
            {
                var actual = await underTest.CanEditTimeFor(testUser.UserId);
                actual.Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task When_Current_User_Is_Editing_There_Own_Time_Then_Allowed()
        {
            foreach (var testUser in TestContext.TestUsers)
            {
                var ctx = new TestContext(x => x.First(x => x.UserId == testUser.UserId));
                var underTest = ctx.GetItemUnderTest();
                var actual = await underTest.CanEditTimeFor(testUser.UserId);

                actual.Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task When_Current_User_Is_Suppervisor_And_Editing_Time_For_Direct_Report_Then_Allowed()
        {
            var ctx = new TestContext(x => x.First(x => x.RoleName == UserRoleName.Supervisor));
            var underTest = ctx.GetItemUnderTest();
            foreach (var testUser in TestContext.TestUsers.Where(x => ctx.CurrentUser.UserId != x.UserId
                                                                      && ctx.CurrentUser.DirectReports.Contains(x.UserId)))
            {
                var actual = await underTest.CanEditTimeFor(testUser.UserId);

                actual.Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task When_Current_User_Is_Suppervisor_And_Editing_Time_For_Non_Direct_Report_Then_Disallowed()
        {
            var ctx = new TestContext(x => x.First(x => x.RoleName == UserRoleName.Supervisor));
            var underTest = ctx.GetItemUnderTest();
            foreach (var testUser in TestContext.TestUsers.Where(x => ctx.CurrentUser.UserId != x.UserId
                                                                      && !ctx.CurrentUser.DirectReports.Contains(x.UserId)))
            {
                var actual = await underTest.CanEditTimeFor(testUser.UserId);

                actual.Should().BeFalse();
            }
        }


    }
}

*/