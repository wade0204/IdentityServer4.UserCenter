using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;

namespace AccessTokenValidationCore
{
    public class CheckUserJtiBlackListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CheckUserJtiBlackListMiddleware> _logger;

        public CheckUserJtiBlackListMiddleware(
            RequestDelegate next,
            ILogger<CheckUserJtiBlackListMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            RedisHelper.Initialization(new CSRedis.CSRedisClient("127.0.0.1:6379,poolsize=20,defaultDatabase=11,db=0,password=123456"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Request.Headers.TryGetValue("Authorization", out var accessToken);
                var accessTokenStr = accessToken.ToString();
                if (!string.IsNullOrWhiteSpace(accessTokenStr) && accessTokenStr.StartsWith(IdentityServerAuthenticationDefaults.AuthenticationScheme))
                {
                    var jti = context.User.FindFirstValue("jti");
                    if (!string.IsNullOrWhiteSpace(jti))
                    {
                        if (await RedisHelper.GetAsync<bool>($"Str_UserJtiBlackList_{jti}"))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }

                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CheckUserJtiBlackListMidddleware异常");
            }
        }
    }
}
