namespace Jizhujian.AspNetCore.Authentication.DingTalk
{
    /// <summary>
    /// Default values for DingTalk authentication
    /// </summary>
    public static class DingTalkDefaults
    {
        /// <summary>
        /// 扫码登录
        /// </summary>
        public const string AuthenticationSchemeQR = "DingTalkQR";
        /// <summary>
        /// 扫码登录
        /// </summary>
        public static readonly string DisplayNameQR = "钉钉扫码";
        /// <summary>
        /// 扫码登录
        /// </summary>
        public static readonly string AuthorizationEndpointQR = "https://oapi.dingtalk.com/connect/qrconnect";
        /// <summary>
        /// 帐号登录
        /// </summary>
        public const string AuthenticationScheme = "DingTalk";
        /// <summary>
        /// 帐号登录
        /// </summary>
        public static readonly string DisplayName = "钉钉";
        /// <summary>
        /// 帐号登录
        /// </summary>
        public static readonly string AuthorizationEndpoint = "https://oapi.dingtalk.com/connect/oauth2/sns_authorize";
        /// <summary>
        /// 用不到 access_token
        /// </summary>
        public static readonly string TokenEndpoint = "https://oapi.dingtalk.com/sns/gettoken";
        /// <summary>
        /// 获取用户信息
        /// </summary>
        public static readonly string UserInformationEndpoint = "https://oapi.dingtalk.com/sns/getuserinfo_bycode";
    }
}