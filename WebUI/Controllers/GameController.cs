using Application.Handlers.Gaming.Join;
using Application.Handlers.Gaming.Play;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebUI.Services;

namespace WebUI.Controllers
{
    public class GameController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IViewRenderService _viewRenderService;

        public GameController(IMediator mediator, IViewRenderService viewRenderService)
        {
            _mediator = mediator;
            _viewRenderService = viewRenderService;
        }

        public async Task<IActionResult> Join()
        {
            var request = new JoinGameRequest();
            var response = await _mediator.Send(request);
            return View("Game", response);
        }

        [HttpPost]
        public async Task<IActionResult> Play(int bet)
        {
            var request = new PlayGameRequest 
            { 
                Bet = bet
            };
            var response = await _mediator.Send(request);
            string partialViewHtml = await _viewRenderService.RenderToString("/Views/Game/_GameResult.cshtml", response);
            return Json(new { partialViewHtml = partialViewHtml, data = response });
        }
    }
}