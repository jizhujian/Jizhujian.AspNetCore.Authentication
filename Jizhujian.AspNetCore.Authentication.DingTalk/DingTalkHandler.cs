using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jizhujian.AspNetCore.Authentication.DingTalk
{
    public class DingTalkHandler : OAuthHandler<DingTalkOptions>
    {
        public DingTalkHandler(IOptionsMonitor<DingTalkOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            return QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"appid", Options.ClientId },
                { "response_type", "code" },
                { "scope",  "snsapi_login" },
                { "state", Options.StateDataFormat.Protect(properties)},
                { "redirect_uri", redirectUri }
            });
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            using var payload = JsonDocument.Parse(JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "access_token", context.Code }
            }));
            return await Task.FromResult(OAuthTokenResponse.Success(payload));
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            // Get the DingTalk user
            var timestamp = GetTimeStamp();
            var requestUri = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"accessKey", Options.ClientId },
                { "timestamp", timestamp },
                { "signature",  Signature(timestamp, Options.ClientSecret) }
            });

            var requestContent = new StringContent(JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "tmp_auth_code", tokens.AccessToken }
            }), Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await Backchannel.PostAsync(requestUri, requestContent, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occurred when retrieving DingTalk user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var payload = JsonDocument.Parse(responseContent);

            int status = payload.RootElement.GetProperty("errcode").GetInt32();
            if (status != 0)
            {
                Logger.LogError("An error occurred while retrieving the user profile: the remote server " +
                                "returned a {Status} response with the following message: {Message}.",
                                /* Status: */ status,
                                /* Message: */  payload.RootElement.GetString("errmsg"));

                throw new HttpRequestException("An error occurred while retrieving user information.");
            }

            var userInfo = payload.RootElement.GetProperty("user_info");
            var principal = new ClaimsPrincipal(identity);
            var context = new OAuthCreatingTicketContext(principal, properties, Context, Scheme, Options, Backchannel, tokens, userInfo);
            context.RunClaimActions();

            await Options.Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);

        }

        private string GetTimeStamp()
        {
            var ts = Clock.UtcNow - DateTimeOffset.UnixEpoch;
            return ts.TotalMilliseconds.ToString("F0", System.Globalization.CultureInfo.InvariantCulture);
        }

        private string Signature(string timestamp, string secret)
        {
            secret ??= string.Empty;
            var encoding = Encoding.UTF8;
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(timestamp);
            using var hmac = HMAC.Create("HMACSHA256");
            hmac.Key = keyBytes;
            byte[] hashMessage = hmac.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashMessage);
        }

    }
}