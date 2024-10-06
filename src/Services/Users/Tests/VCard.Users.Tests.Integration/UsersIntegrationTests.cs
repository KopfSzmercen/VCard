using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MassTransit.Testing;
using VCard.Users.Api.Presentation;
using VCard.Users.IntegrationEvents;
using VCard.Users.Tests.Integration.Setup;
using VCard.Common.Infrastructure.Consumers;


namespace VCard.Users.Tests.Integration;

public class UsersIntegrationTests : UserIntegrationTestsBase
{
    [Fact]
    public async Task GivenEmailIsUnique_RegisterUser_ShouldSucceed()
    {
        // Arrange
        var app = App.WithTestEventBus(
            typeof(IntegrationEventConsumer<UserRegistered>)
        );

        var client = app.CreateClient();

        var request = new RegisterUserEndpoint.Request
        {
            Email = "test@t.pl",
            Password = "password"
        };

        // Act
        var response = await client.PostAsJsonAsync(UsersEndpoints.BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var testHarness = app.Services.GetTestHarness();

        testHarness.Consumed.Select<UserRegistered>().Any().Should().BeTrue();
        testHarness.Published.Select<UserRegistered>().Any().Should().BeTrue();
    }

    [Fact]
    public async Task GivenValidSignInWithCookie_UserShouldAccessProtectedRoute()
    {
        // Arrange
        var client = App.WithTestEventBus().CreateClient();

        var request = new RegisterUserEndpoint.Request
        {
            Email = "test@t.pl",
            Password = "password"
        };

        await client.PostAsJsonAsync(UsersEndpoints.BaseUrl, request);

        var signInRequest = new SignInUserEndpoint.Reuqest
        {
            Email = "test@t.pl",
            Password = "password"
        };

        // Act
        var signInResponse = await client.PostAsJsonAsync($"{UsersEndpoints.BaseUrl}/sign-in", signInRequest);
        signInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getMeResponse = await client.GetAsync($"{UsersEndpoints.BaseUrl}/me");

        var getMeResponseContent = await getMeResponse.Content.ReadFromJsonAsync<GetMeEndpoint.Response>();

        // Assert
        getMeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getMeResponseContent!.Email.Should().Be(signInRequest.Email);
    }

    [Fact]
    public async Task GivenValidSignInWithJwt_UserShouldAccessProtectedRoute()
    {
        // Arrange
        var client = App.WithTestEventBus().CreateClient();

        var request = new RegisterUserEndpoint.Request
        {
            Email = "test@t.pl",
            Password = "password"
        };

        await client.PostAsJsonAsync(UsersEndpoints.BaseUrl, request);

        var signInRequest = new SignInUserEndpoint.Reuqest
        {
            Email = "test@t.pl",
            Password = "password"
        };

        // Act
        var signInResponse = await client.PostAsJsonAsync($"{UsersEndpoints.BaseUrl}/sign-in-jwt", signInRequest);
        signInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var signInResponseContent = await signInResponse.Content.ReadFromJsonAsync<SignInUserJwtEndpoint.Response>();

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {signInResponseContent!.Token}");

        var getMeResponse = await client.GetAsync($"{UsersEndpoints.BaseUrl}/me");

        var getMeResponseContent = await getMeResponse.Content.ReadFromJsonAsync<GetMeEndpoint.Response>();

        // Assert
        getMeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getMeResponseContent!.Email.Should().Be(signInRequest.Email);
    }
}