using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Jizhujian.AspNetCore.Authentication.DingTalk
{
    /// <summary>
    /// Configuration options for <see cref="DingTalkHandler"/>.
    /// </summary>
    public class DingTalkOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="DingTalkOptions"/>.
        /// </summary>
        public DingTalkOptions()
        {
            CallbackPath = new PathString("/signin-dingtalkqr");
            AuthorizationEndpoint = DingTalkDefaults.AuthorizationEndpointQR;
            TokenEndpoint = DingTalkDefaults.TokenEndpoint;
            UserInformationEndpoint = DingTalkDefaults.UserInformationEndpoint;

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "nick");
            ClaimActions.MapJsonKey("urn:dingtalk:unionid", "unionid");
        }
    }
}