using System;
using Jizhujian.AspNetCore.Authentication.DingTalk;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Jizhujian.Extensions.DependencyInjection
{
    public static class DingTalkExtensions
    {
        /// <summary>
        /// 钉钉扫码登录：
        ///     .AddDingTalk(DingTalkDefaults.AuthenticationSchemeQR, DingTalkDefaults.DisplayNameQR, options =>
        ///     {
        ///         options.ClientId = "";
        ///         options.ClientSecret = "";
        ///         // options.AuthorizationEndpoint = DingTalkDefaults.AuthorizationEndpointQR;
        ///         // options.CallbackPath = new PathString("/signin-dingtalkqr");
        ///     })
        /// 钉钉帐号登录：
        ///     .AddDingTalk(DingTalkDefaults.AuthenticationScheme, DingTalkDefaults.DisplayName, options =>
        ///     {
        ///         options.ClientId = "";
        ///         options.ClientSecret = "";
        ///         options.AuthorizationEndpoint = DingTalkDefaults.AuthorizationEndpoint;
        ///         options.CallbackPath = new PathString("/signin-dingtalk");
        ///     })
        /// </summary>
        public static AuthenticationBuilder AddDingTalk(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<DingTalkOptions> configureOptions)
            => builder.AddOAuth<DingTalkOptions, DingTalkHandler>(authenticationScheme, displayName, configureOptions);
    }
}