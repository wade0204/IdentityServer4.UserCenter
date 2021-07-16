using Microsoft.AspNetCore.Builder;

namespace AccessTokenValidationCore
{
    public static class AuthenticationUseExtension
    {
        public static IApplicationBuilder AddUserCenterAuthentication(this IApplicationBuilder app)
        {
            return app.UseAuthentication()
                .UseAuthorization()
                .UseMiddleware<CheckUserJtiBlackListMiddleware>();
        }
    }
}
