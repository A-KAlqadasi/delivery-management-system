using FluentValidation;

namespace Delivery.Features.Zones.CreateZone;

internal sealed class CreateZoneCommandValidator : AbstractValidator<CreateZoneCommand>
{
	public CreateZoneCommandValidator()
	{
		RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
		RuleFor(c => c.DeliveryFee).GreaterThanOrEqualTo(0).When(c => c.DeliveryFee.HasValue);
	}
}
