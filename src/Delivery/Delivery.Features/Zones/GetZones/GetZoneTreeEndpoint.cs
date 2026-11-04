using Delivery.Domain;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Modules.Common.Api.Abstractions;
using Modules.Common.Api.Extensions;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;

namespace Delivery.Features.Zones.GetZones;

public sealed class GetZoneTreeEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet("/api/zones", Handle)
		.RequireAuthorization(Permissions.ReadZone)
		.WithTags(Tags.Zones);
	}

	private static async Task<IResult> Handle(ISender sender)
	{
		Result<List<GetZonesResponse>> result = await sender.Send(new GetZoneTreeQuery());

		return result.Match(Results.Ok, e => e.ToProblem());
	}

}

