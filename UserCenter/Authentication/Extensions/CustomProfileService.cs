using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserCenter.Common;

namespace Authentication.Extensions
{
    /// <summary>
    /// refreshToken 添加claims
    /// </summary>
    public class CustomProfileService : IProfileService
    {
        public CustomProfileService()
        {
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var isLogin = context.Subject.Claims.FirstOrDefault(p => p.Type == ExClaimTypesConst.IsLogin)?.Value;
            List<Claim> claims;
            if (!string.IsNullOrWhiteSpace(isLogin))
            {
                claims = context.Subject.Claims.Where(p => p.Type != ExClaimTypesConst.IsLogin).ToList();
                context.IssuedClaims.AddRange(claims);
                return Task.CompletedTask;
            }

            var jti = context.Subject.Claims.FirstOrDefault(p => p.Type == JwtClaimTypes.JwtId)?.Value;
            var userIdStr = context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrWhiteSpace(userIdStr))
            {
                context.IssuedClaims.AddRange(context.Subject.Claims);
                return Task.CompletedTask;
            }

            claims = context.Subject.Claims.ToList();
            claims.Add(new Claim(ExClaimTypesConst.IsRefresh, jti));
            context.IssuedClaims.AddRange(claims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
