
using UnitConversion.Api.ErrorHandling;
using UnitConversion.Application;
using UnitConversion.Application.Conversions;
using UnitConversion.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<UnitConversionExceptionHandler>();

builder.Services
    .AddOptions<ConversionOptions>()
    .Bind(builder.Configuration.GetSection(ConversionOptions.SectionName))
    .Validate(
        options => options.DecimalPlaces is >= 0 and <= 28,
        "Conversion:DecimalPlaces must be between 0 and 28.")
    .ValidateOnStart();

builder.Services
    .AddApplication()
    .AddInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Unit Conversion API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
