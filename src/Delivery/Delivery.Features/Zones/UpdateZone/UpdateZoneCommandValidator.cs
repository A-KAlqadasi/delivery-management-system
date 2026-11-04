using FluentValidation;

namespace Delivery.Features.Zones.UpdateZone;

internal sealed class UpdateZoneCommandValidator : AbstractValidator<UpdateZoneCommand>
{
    public UpdateZoneCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.DeliveryFee).GreaterThanOrEqualTo(0).When(c => c.DeliveryFee.HasValue);
    }
}
