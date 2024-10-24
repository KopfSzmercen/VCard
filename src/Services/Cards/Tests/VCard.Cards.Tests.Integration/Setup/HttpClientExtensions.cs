﻿using System.Net.Http.Headers;
using System.Security.Claims;
using VCard.Common.Auth.TokensManager;

namespace VCard.Cards.Tests.Integration.Setup;

internal static class HttpClientExtensions
{
    public static void Authenticate(this HttpClient client, Guid userId, ITokensManager tokensManager)
    {
        var token = tokensManager.CreateToken(
            userId,
            [],
            [
                new Claim(ClaimTypes.Name, userId.ToString()),
                new Claim("ManageCards", true.ToString())
            ]
        );

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    }
}