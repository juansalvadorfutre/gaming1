using FluentValidation;

namespace Application.Handlers.Gaming.Play
{
    public class PlayGameRequestValidator : AbstractValidator<PlayGameRequest>
    {
        public PlayGameRequestValidator()
        {
            RuleFor(v => v.Bet)
                .NotNull();
        }
    }
}
