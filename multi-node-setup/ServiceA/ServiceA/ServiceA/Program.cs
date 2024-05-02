using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services);

var app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);


// Start web application
app.Run();

void ConfigureServices(IServiceCollection services)
{
	services.AddControllers(options => options.ReturnHttpNotAcceptable = true)
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
			options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});
	
	services.AddRouting(options => options.LowercaseUrls = true);

	services.AddAutoMapper(typeof(Program));

	services.AddEndpointsApiExplorer();
	services.AddSwaggerGen();

	services.AddCors(builder =>
		builder.AddDefaultPolicy(policy =>
		{
			policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
		}));
	
	services.AddOpenTelemetry()
	.WithTracing(b =>
	{
		b
			.AddSource(builder.Configuration["ServiceName"] ?? throw new ArgumentException("ServiceName must not be empty"))
			.ConfigureResource(resource =>
				resource.AddService(
					serviceName: builder.Configuration["ServiceName"] ?? throw new ArgumentException("ServiceName must not be empty"),
					serviceVersion: builder.Configuration["ServiceVersion"]))
			.AddAspNetCoreInstrumentation()
			.AddHttpClientInstrumentation()
			.AddOtlpExporter(options => options.Endpoint = new Uri("http://fluentbit-forwarder:4318"));
	});

	services.AddHttpClient("ServiceBClient", client =>
	{
		client.BaseAddress = new Uri(builder.Configuration["SERVICEB_URL"] ?? throw new ArgumentException("SERVICEB_URL must not be set"));
	});
}

void ConfigureMiddleware(IApplicationBuilder app)
{
	app.UseHttpsRedirection();

	app.UseAuthentication();
	app.UseAuthorization();

	app.UseSwagger();
	app.UseSwaggerUI();

	app.UseCors();
}

void ConfigureEndpoints(IEndpointRouteBuilder app)
{
	app.MapControllers();
}