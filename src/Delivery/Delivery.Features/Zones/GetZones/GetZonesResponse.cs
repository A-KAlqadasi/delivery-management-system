namespace Delivery.Features.Zones.GetZones;

internal sealed record GetZonesResponse(
    Guid Id,
    string Name,
    decimal? DeliveryFee,
    IReadOnlyList<GetZonesResponse> Children);

