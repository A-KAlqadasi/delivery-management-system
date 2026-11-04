
using Delivery.Domain;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Modules.Common.Api.Abstractions;
using Modules.Common.Api.Extensions;
using Modules.Common.Application.MediatR;

namespace Delivery.Features.Zones.CreateZone;

public sealed class CreateZoneEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost("/api/zones", async (CreateZoneCommand request, ISender sender) =>
		{
			var result = await sender.Send(request);
			return result.Match(
				id => Results.Created($"/api/zones/{id}", id),
				errors => errors.ToProblem());
		})
		.RequireAuthorization(Permissions.CreateZone)
		.WithTags(Tags.Zones);
	}
}



