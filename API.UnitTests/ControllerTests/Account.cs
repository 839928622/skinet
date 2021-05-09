using System.Threading;
using System.Threading.Tasks;
using API.Controllers;
using API.Dtos;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace API.UnitTests.ControllerTests
{
    //https://softwareengineering.stackexchange.com/questions/7823/is-it-ok-to-have-multiple-asserts-in-a-single-unit-test
    // model validation tests is gonna be practice on integration tests
    public class Account
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _autoMapperMock;

        public Account()
        {
            _autoMapperMock = new Mock <IMapper>();
            _tokenServiceMock = new Mock<ITokenService>();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

           // https://stackoverflow.com/questions/48189741/mocking-a-signinmanager
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object, 
                Mock.Of<IHttpContextAccessor>(), 
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null,null,null,null);
          
        }
        // test should failed for one reason only
        [Fact]
        public async Task LoginPost_ReturnsOK_WhenPassingCorrectCredential()
        {
            // Arrange
            var fakeUser = new ApplicationUser()
            {
                Email = "fake@email.com",
                DisplayName = "fakeDisplayName"
            };
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(fakeUser);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);
            _tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<ApplicationUser>())).Returns(It.IsAny<string>());
            var controller = new AccountController(_userManagerMock.Object,_signInManagerMock.Object,_tokenServiceMock.Object,_autoMapperMock.Object);
            
            // Act
            var result = await controller.Login(new LoginDto()
            {
                Email = "a@b.com",
                Password = "password"
            });

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
           
        }

        [Fact]
        public async Task LoginPost_ReturnsUnauthorized_WhenPassingInvalidCredential()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync( (ApplicationUser) null);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _tokenServiceMock.Object, _autoMapperMock.Object);

            // Act
            var result = await controller.Login(new LoginDto()
            {
                Email = "a@b.com",
                Password = "password"
            });

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
           
        }

        [Fact]
        public async Task LoginPost_ReturnsUnauthorized_WhenPassingIncorrectPassword()
        {
            // Arrange
            var fakeUser = new ApplicationUser()
            {
                Email = "fake@email.com",
                DisplayName = "fakeDisplayName"
            };
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(fakeUser);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Failed);

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _tokenServiceMock.Object, _autoMapperMock.Object);

            // Act
            var result = await controller.Login(new LoginDto()
            {
                Email = "a@b.com",
                Password = "password"
            });

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);

        }

        [Fact]
      public  async Task RegisterPost_ReturnBadRequest_WhenEmailAlreadyInUse()
        {
            //Arrange
            var fakeRegisterDto = new RegisterDto()
            {
                Email = "a@b.com",
            };
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _tokenServiceMock.Object, _autoMapperMock.Object);

            //Act
            var res = await controller.Register(fakeRegisterDto);
            //Assert
            Assert.IsType<BadRequestObjectResult>(res.Result);
        }

     

    }
}
