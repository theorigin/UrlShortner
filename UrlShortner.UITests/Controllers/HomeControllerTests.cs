using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using UrlShortner.DataAccess;
using UrlShortner.UI.Controllers;
using Xunit;

namespace UrlShortner.UITests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IUrlRepository> _mockUrlRepository;
        private readonly HomeController _controller;
        private readonly TempDataDictionary _tempData;
        private readonly DefaultHttpContext _httpContext;
        private readonly Mock<IUrlHelper> _urlHelper;

        public HomeControllerTests()
        {
            _urlHelper = new Mock<IUrlHelper>();
            
            _httpContext = new DefaultHttpContext();
            _tempData = new TempDataDictionary(_httpContext, Mock.Of<ITempDataProvider>());
            _mockUrlRepository = new Mock<IUrlRepository>();
            _controller = new HomeController(_mockUrlRepository.Object)
            {
                TempData = _tempData,
                Url = _urlHelper.Object
            };
        } 

        [Fact]
        public void Index_Null_Id_Returns_Correct_View()
        {
            var viewResult = _controller.Index(string.Empty)
                .Should()
                .BeOfType<ViewResult>()
                .Subject;

            viewResult.ViewName.Should().Be("Index");
        }

        [Fact]
        public void Index_Unknown_Id_Returns_NotFound_View()
        {
            _mockUrlRepository.Setup(x => x.GetById(It.IsAny<string>())).Returns(string.Empty);
            var viewResult = _controller.Index("123")
                .Should()
                .BeOfType<ViewResult>()
                .Subject;

            viewResult.ViewName.Should().Be("NotFound");
        }

        [Fact]
        public void Index_Known_Id_Returns_Redirect_To_Original_Url()
        {
            const string? originalUrl = "http://original-url.com";
            _mockUrlRepository.Setup(x => x.GetById("123")).Returns(originalUrl);
            var viewResult = _controller.Index("123")
                .Should()
                .BeOfType<RedirectResult>()
                .Subject;

            viewResult.Url
                .Should()
                .Be(originalUrl);
            viewResult.Permanent
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Create_Calls_UrlRepository_Save()
        {
            const string originalUrl = "http://original-url.com";
            
            _controller.Create(originalUrl)
                .Should()
                .BeOfType<RedirectToActionResult>();

            _mockUrlRepository.Verify(x => x.Save(It.IsAny<string>(), originalUrl), Times.Once);
        }

        [Fact]
        public void Create_Sets_TempData_To_Link()
        {
            const string originalUrl = "http://original-url.com";
            var generatedId = "";

            _mockUrlRepository.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string id, string url) => { generatedId = id; });
            _urlHelper.Setup(x => x.Link("index", It.IsAny<object>())).Returns((string url, object values) => $"{values}");

            _controller.Create(originalUrl);

            _controller.TempData["IndexLink"]?.ToString()
                .Should()
                .Contain(generatedId);
        }

        [Fact]
        public void Create_Returns_RedirectAction_To_Index()
        {
            const string originalUrl = "http://original-url.com";

            var viewResult = _controller.Create(originalUrl)
                .Should()
                .BeOfType<RedirectToActionResult>()
                .Subject;

            viewResult.ActionName
                .Should()
                .Be("Index");

            viewResult.Permanent
                .Should()
                .BeFalse();
        }
    }
}