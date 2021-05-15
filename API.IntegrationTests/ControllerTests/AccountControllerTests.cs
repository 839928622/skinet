using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.IntegrationTests.Helpers;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace API.IntegrationTests.ControllerTests
{
    public class AccountControllerTests : TestBase, IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _autoMapperMock;
        /// <inheritdoc />
        public AccountControllerTests(TestApplicationFactory<Startup, FakeStartup> factory, ITestOutputHelper testOutputHelper
            ) : base(factory)
        {
            _testOutputHelper = testOutputHelper;
            _autoMapperMock = new Mock<IMapper>();
            _tokenServiceMock = new Mock<ITokenService>();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // https://stackoverflow.com/questions/48189741/mocking-a-signinmanager
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);
        }

        [Theory]
        [InlineData("api/Account")]
        [InlineData("api/Account/address")]
        public async Task Get_EndpointsReturnFailToAnonymousUserForRestrictedUrls(string url)
        {
            // Arrange
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            // Act
            var response = await client.GetAsync(url);


            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        [Theory]
        [InlineData("api/Account/address")]
        public async Task Put_UpdateUserAddressReturnFailToAnonymousUserForRestrictedUrls(string url)
        {
            // Arrange
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new AddressDto()), Encoding.UTF8,
                "application/json");
            // Act
            var response = await client.PutAsync(url, content);
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #region Register

        [Fact]
        public async Task Post_Register_With_Empty_Email_Should_Return_Bad_Request()
        {
            // Arrange
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var dto = new RegisterDto()
            {
                DisplayName = "display",
                Email = string.Empty,
                Password = "Passwo$d",
            };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(dto), Encoding.UTF8,
                "application/json");
            // Act
            var response = await client.PostAsync("api/Account/register", content);

            var res = await response.Content.ReadFromJsonAsync<APiResponse>();
            Assert.NotNull(res);
            Assert.Equal("you have made a bad request", res.Message);
            //_testOutputHelper.WriteLine(res);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

        #endregion
        [Fact]
        public async Task Get_Get_Current_User_With_Auth_Should_Return_OK()
        {
            // Arrange
            var identityDbContext = Factory.Services.GetRequiredService<AppIdentityDbContext>();
            if (!identityDbContext.Users.Any())
            {
                var userManager = Factory.Services.GetRequiredService<UserManager<ApplicationUser>>();
                var user = new ApplicationUser()
                {
                    UserName = "test",
                    DisplayName = "test",
                    Email = "testuser@mail.com",
                    Address = new Address()
                    {
                        FirstName = "test FirstName",
                        LastName = "test LastName",
                        State = "test State",
                        Street = "test Street",
                        Zipcode = "test Zipcode"
                    }
                };
                await userManager.CreateAsync(user, "Pa$$w0rd");

            }
            var claimsProvider = TestClaimsProvider.WithUserClaims();
            var client = Factory.CreateClientWithTestAuth(claimsProvider);

            // Act
            var response = await client.GetAsync("api/Account");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            var identityDbContext = Factory.Services.GetRequiredService<AppIdentityDbContext>();
            var userManager = Factory.Services.GetRequiredService<UserManager<ApplicationUser>>();
            var user = identityDbContext.Users.FirstOrDefault(u => u.UserName == "test");
            if (user == null) return;
            identityDbContext.Users.Remove(user);
            identityDbContext.SaveChanges();
            _testOutputHelper.WriteLine("test user removed");
        }
    }
}
