using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;

namespace Authentication
{
    public class Config
    {
        public const string defaltClientSecret = "shc%hD#2!shG&";

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        /// <summary>
        /// 定义要保护的资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new("userCenter","MyApi")
            };
        }

        /// <summary>
        /// 定义授权客户端
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new()
                {
                    ClientId = "client",
                    AllowedGrantTypes = new List<string>() { GrantTypes.ResourceOwnerPassword.FirstOrDefault(), "anonymous" },
                    // 主要刷新refresh_token
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 60 * 60 * 24 , //1天
                    AbsoluteRefreshTokenLifetime = 60 * 60 * 24 * 30, //30天
                    ClientSecrets =
                    {
                        new Secret(defaltClientSecret.Sha256())
                    },
                    RequireClientSecret = true,
                    AllowedScopes =
                    { 
                        //如果要获取refresh_tokens ,必须在scopes中加上OfflineAccess
                        "userCenter",
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    IncludeJwtId = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RefreshTokenUsage = TokenUsage.ReUse
                }
            };
        }
    }
}
