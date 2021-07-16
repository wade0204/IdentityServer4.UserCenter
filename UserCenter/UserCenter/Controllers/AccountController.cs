using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserCenter.Common;
using UserCenter.Common.Request;
using UserCenter.Common.Response;
using UserCenter.IService;

namespace UserCenter.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    public class AccountController :BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            AppSettings appSettings,
            IAccountService accountService,
            ILogger<AccountController> logger)
        {
            _logger = logger;
            _appSettings = appSettings;
            _accountService = accountService;
            //_endpointHandler = endpointHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<BaseResponse<ResLoginModel>> LogIn([FromBody] ReqLoginModel loginModel)
        {
            return await _accountService.Login(loginModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("LogOut")]
        public async Task<BaseResponse<bool>> LogOut()
        {
            return await _accountService.LogOut(UserId, Jti);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpGet("RefreshToken")]
        [AllowAnonymous]
        public async Task<BaseResponse<ResLoginModel>> RefreshToken(string refreshToken)
        {
            return await _accountService.RefreshToken(refreshToken);
        }
    }
}
