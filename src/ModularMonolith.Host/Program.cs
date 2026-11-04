using Delivery.Infrastructure;

using ModularMonolith.Host;
using ModularMonolith.Host.Extensions;
using ModularMonolith.Host.Middlewares;

using Modules.Common.Api.Extensions;
using Modules.Users.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

// modules configuration

// Reuse common infrastructure for web API, authentication, etc.
builder.Services.AddCoreWebApiInfrastructure();

//builder.Configuration.AddModuleConfiguration(["users"]);

builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddDeliveryModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();

	app.MapOpenApi();

	// app.UseSwaggerUI(options =>
	// {
	// 	options.SwaggerEndpoint("/openapi/v1.json", "Delivery management system API");

	// 	options.EnableDeepLinking();
	// 	options.DisplayRequestDuration();
	// 	options.EnableFilter();
	// });

	await app.ApplyMigrations();

}
// global middlewares
app.UseCoreMiddlewares(app.Environment);

// modules endpoints
app.MapApiEndpoints();

await app.RunAsync();
