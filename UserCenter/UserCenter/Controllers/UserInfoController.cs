using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserCenter.Common.Entity;
using UserCenter.IService;

namespace UserCenter.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    public class UserInfoController : BaseController
    {
        private readonly ILogger<UserInfoController> _logger;
        private readonly IUserInfoService _userInfoService;

        public UserInfoController(ILogger<UserInfoController> logger,
            IUserInfoService userInfoService)
        {
            _logger = logger;
            _userInfoService = userInfoService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetUserInfo")]
        public async Task<UserInfoEntity> GetUserInfo(long id)
        {
            return new ()
            {
                Id = UserId,
                UserName = UserName
            };
        }
    }
}
