using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using API.Dtos;
using API.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.IntegrationTests.ControllerTests
{
    public class AccountControllerTests : TestBase
    {
        /// <inheritdoc />
        public AccountControllerTests(TestApplicationFactory<Startup, FakeStartup> factory) : base(factory)
        {
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
    }
}
