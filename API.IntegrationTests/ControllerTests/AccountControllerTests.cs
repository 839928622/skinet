using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using API.Dtos;
using API.IntegrationTests.Helpers;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace API.IntegrationTests.ControllerTests
{
    public class AccountControllerTests : TestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _autoMapperMock;
        /// <inheritdoc />
        public AccountControllerTests(TestApplicationFactory<Startup, FakeStartup> factory, ITestOutputHelper testOutputHelper) : base(factory)
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

            var res = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine(res);
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

        #endregion
        [Fact]
        public async Task Get_Get_Current_User_With_Auth_Shoud_Return_OK()
        {
            // Arrange
            var claimsProvider = TestClaimsProvider.WithUserClaims();
            var client = Factory.CreateClientWithTestAuth(claimsProvider);


        }
    }
}
