using System.Threading.Tasks;
using Application.Handlers.Gaming.Join;
using Application.Handlers.Gaming.Play;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebUI.Controllers;
using WebUI.Services;
using Xunit;

namespace UnitTests.WebUI.Controllers
{
    public class GameControllerShould
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IViewRenderService> _mockViewRenderService;
        private readonly GameController _sut;

        public GameControllerShould()
        {
            _mockMediator = new Mock<IMediator>();
            _mockViewRenderService = new Mock<IViewRenderService>();
            _sut = new GameController(_mockMediator.Object, _mockViewRenderService.Object);
        }

        [Fact]
        public async Task Return_View_With_Response_When_Calling_Join_Action()
        {
            var request = new JoinGameRequest();
            var response = new JoinGameResponse { Min = "1", Max = "100" };
            _mockMediator
                .Setup(m => m.Send(request, default))
                .ReturnsAsync(response);

            var result = await _sut.Join();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Game", viewResult.ViewName);
            Assert.Equal(response, viewResult.Model);
        }

        [Fact]
        public async Task Return_Json_With_Partial_ViewHtml_And_Data_When_Calling_Play_Action()
        {
            // Arrange
            var request = new PlayGameRequest { Bet = 10 };
            var response = new PlayGameResponse();
            _mockMediator
                .Setup(m => m.Send(request, default))
                .ReturnsAsync(response);

            var partialViewHtml = "<div>Game Result: Win</div>";
            _mockViewRenderService
                .Setup(v => v.RenderToString("/Views/Game/_GameResult.cshtml", response))
                .ReturnsAsync(partialViewHtml);

            _sut.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _sut.Play(10);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<PlayGameResponse>(jsonResult.Value.GetType().GetProperty("data").GetValue(jsonResult.Value));
            Assert.Equal(response, data);

            var actualPartialViewHtml = jsonResult.Value.GetType().GetProperty("partialViewHtml").GetValue(jsonResult.Value);
            Assert.Equal(partialViewHtml, actualPartialViewHtml);

            _mockMediator.Verify(m => m.Send(request, default), Times.Once);
            _mockViewRenderService.Verify(v => v.RenderToString("/Views/Game/_GameResult.cshtml", response), Times.Once);
        }
    }
}
