using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversion.Api.Contracts;

namespace UnitConversion.Tests.Api;

public class ConversionsApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_ReturnsConvertedValue()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=100&from=C&to=F");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.NotNull(body);
        Assert.Equal("Temperature", body.Category);
        Assert.Equal("C", body.From);
        Assert.Equal("F", body.To);
        Assert.Equal(212m, body.Result, 8);
    }

    [Fact]
    public async Task Post_ReturnsConvertedValue()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/conversions",
            new { value = 1, from = "kg", to = "lb" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.NotNull(body);
        Assert.Equal(2.2046226218m, body.Result, 8);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundProblem_ForUnknownUnit()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=1&from=parsec&to=m");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_ForIncompatibleUnits()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=1&from=m&to=kg");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_ForTemperatureBelowAbsoluteZero()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=-300&from=C&to=K");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/conversions?from=m&to=ft")]
    [InlineData("/api/v1/conversions?value=1&to=ft")]
    [InlineData("/api/v1/conversions?value=1&from=m")]
    [InlineData("/api/v1/conversions?value=abc&from=m&to=ft")]
    public async Task Get_ReturnsBadRequest_ForMissingOrInvalidParameters(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Units_ReturnsAllUnits()
    {
        var units = await _client.GetFromJsonAsync<List<UnitResponse>>("/api/v1/units");

        Assert.NotNull(units);
        Assert.NotEmpty(units);
    }

    [Fact]
    public async Task Units_CanBeFilteredByCategory()
    {
        var units = await _client.GetFromJsonAsync<List<UnitResponse>>("/api/v1/units?category=mass");

        Assert.NotNull(units);
        Assert.NotEmpty(units);
        Assert.All(units, unit => Assert.Equal("Mass", unit.Category));
    }

    [Fact]
    public async Task Units_ReturnsBadRequest_ForUnknownCategory()
    {
        var response = await _client.GetAsync("/api/v1/units?category=volume");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
