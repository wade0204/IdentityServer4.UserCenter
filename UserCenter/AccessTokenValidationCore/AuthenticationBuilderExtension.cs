using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AccessTokenValidationCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuthenticationBuilderExtension
    {
        private const string AuthenticationScheme = "introspection";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddUserCenterBearerAuthentication(
            this IServiceCollection builder, UserCenterAuthticationOption option)
        {
            return builder.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = option.Authority; //授权服务器地址
                    options.RequireHttpsMetadata = true; //不需要https
                    options.Audience = option.ClientScope;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = false
                    };
                    options.ForwardDefaultSelector = Selector.ForwardReferenceToken(AuthenticationScheme);
                })
                .AddOAuth2Introspection(AuthenticationScheme, options =>
                {
                    options.Authority = option.Authority; //授权服务器地址
                    options.ClientId = option.ClientId;
                    options.ClientSecret = option.ClientSecret;
                });
        }
    }
}
