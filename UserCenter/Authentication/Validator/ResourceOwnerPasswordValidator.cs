using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UserCenter.Common;
using UserCenter.Common.Request;
using UserCenter.IService;

namespace Authentication.Validator
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
        private readonly IUserInfoService _userInfoService;
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfoService"></param>
        /// <param name="contextAccessor"></param>
        /// <param name="logger"></param>
        public ResourceOwnerPasswordValidator(
            IUserInfoService userInfoService,
            IHttpContextAccessor contextAccessor, 
            ILogger<ResourceOwnerPasswordValidator> logger)
        {
            _logger = logger;
            _userInfoService = userInfoService;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UserName))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "请输入用户名");
                return;
            }
            if (string.IsNullOrWhiteSpace(context.Password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "请输入密码");
                return;
            }

            try
            {
                //用户登录验证
                var userInfo = await _userInfoService.Login(new ReqLoginModel()
                {
                    Username = context.UserName,
                    Password = context.Password
                });
                if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.UserName))
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户名密码错误");
                    return;
                }

                var extra = new Dictionary<string, object> {{"user", userInfo}};
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, userInfo.UserName),
                    new(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                    new(ExClaimTypesConst.IsLogin, ExClaimTypesConst.IsLogin)
                };
                //add jti
                var jti = CryptoRandom.CreateUniqueId(16);
                if (_contextAccessor.HttpContext != null) 
                    _contextAccessor.HttpContext.Items[JwtClaimTypes.JwtId] = jti;

                var tokenResult = new GrantValidationResult(userInfo.Id.ToString(),
                    GrantType.ResourceOwnerPassword, claims, customResponse: extra);
                context.Result = tokenResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "平台登录异常");
            }
        }
    }
}
