using Delivery.Domain;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Modules.Common.Api.Abstractions;
using Modules.Common.Api.Extensions;
using Modules.Common.Application.MediatR;

namespace Delivery.Features.Zones.UpdateZone;

public sealed class UpdateZoneEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/api/zones/{id:guid}", Handle)
        .RequireAuthorization(Permissions.UpdateZone)
        .WithTags(Tags.Zones);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] UpdateZoneCommand request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return Results.BadRequest();
        }

        var result = await sender.Send(request, cancellationToken);
        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblem());
    }
}
