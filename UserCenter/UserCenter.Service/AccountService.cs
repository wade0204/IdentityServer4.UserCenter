using Autofac.AttributeExtensions;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UserCenter.Common;
using UserCenter.Common.Const;
using UserCenter.Common.Helper;
using UserCenter.Common.Request;
using UserCenter.Common.Response;
using UserCenter.IService;

namespace UserCenter.Service
{
    /// <summary>
    /// 
    /// </summary>
    [InstancePerLifetimeScope]
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _clientFactory;

        public AccountService(
            AppSettings appSettings,
            ILogger<AccountService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _appSettings = appSettings;
            _clientFactory = httpClientFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        public async Task<BaseResponse<ResLoginModel>> Login(ReqLoginModel loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel?.Username) || string.IsNullOrWhiteSpace(loginModel?.Password))
                return BaseResponse<ResLoginModel>.Error("用户名密码错误");

            try
            {
                var client = _clientFactory.CreateClient();
                var disco = await GetDiscoveryDocumentAsync(
                    client, _appSettings.Ids4Config.Authority);
                if (disco.IsError)
                    return BaseResponse<ResLoginModel>.Error("Identity服务发现异常，请稍后再试！");

                var res = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _appSettings.Ids4Config.ClientId,
                    ClientSecret = _appSettings.Ids4Config.ClientSecret,
                    Scope = _appSettings.Ids4Config.ClientScope,
                    GrantType = _appSettings.Ids4Config.GrantType,
                    UserName = loginModel.Username,
                    Password = loginModel.Password.Md5Encrypt32()
                });

                return res.IsError ? BaseResponse<ResLoginModel>.Error(res.Error) :
                    BaseResponse<ResLoginModel>.Success(JsonConvert.DeserializeObject<ResLoginModel>(res.Raw));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "登录异常", loginModel);

                return BaseResponse<ResLoginModel>.Error("登录异常，请稍后再试！");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> LogOut(long userId, string jti)
        {
            if (string.IsNullOrWhiteSpace(jti) || userId <= 0)
                return BaseResponse<bool>.Error("退出登录失败，请稍后再试！");

            try
            {
                return await RedisHelper.SetAsync($"{RedisKeyConst.Str_UserJtiBlackList}_{jti}", true, TimeSpan.FromDays(1))
                    ? BaseResponse<bool>.Success()
                    : BaseResponse<bool>.Error("退出登录失败，请稍后再试！");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"退出登录异常,userId:{userId}, jti:{jti}");

                return BaseResponse<bool>.Error("退出登录异常，请稍后再试！");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<BaseResponse<ResLoginModel>> RefreshToken(string refreshToken)
        {
            if(string.IsNullOrWhiteSpace(refreshToken))
                return BaseResponse<ResLoginModel>.Error("刷新令牌不能为空！");

            try
            {
                var client = _clientFactory.CreateClient();
                var disco = await GetDiscoveryDocumentAsync(
                    client, _appSettings.Ids4Config.Authority);
                if (disco.IsError)
                    return BaseResponse<ResLoginModel>.Error("Identity服务发现异常，请稍后再试！");

                var res = await client.RequestRefreshTokenAsync(new RefreshTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _appSettings.Ids4Config.ClientId,
                    ClientSecret = _appSettings.Ids4Config.ClientSecret,
                    Scope = _appSettings.Ids4Config.ClientScope,
                    GrantType = _appSettings.Ids4Config.GrantTypeRefreshToken,
                    RefreshToken = refreshToken
                });

                return res.IsError ? BaseResponse<ResLoginModel>.Error(res.Error) :
                    BaseResponse<ResLoginModel>.Success(JsonConvert.DeserializeObject<ResLoginModel>(res.Raw));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "刷新令牌异常", refreshToken);

                return BaseResponse<ResLoginModel>.Error("刷新令牌异常，请稍后再试！");
            }
        }

        /// <summary>
        /// Identity服务发现
        /// </summary>
        /// <param name="client"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(HttpClient client, string address)
        {
            try
            {
                return await client.GetDiscoveryDocumentAsync(address);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Identity服务发现异常", address);

                return null;
            }
        }
    }
}
