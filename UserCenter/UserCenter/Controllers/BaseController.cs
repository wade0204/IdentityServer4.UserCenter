using Authentication.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserCenter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("v{version:apiVersion}/[controller]")]
    public class BaseController : ControllerBase
    {
        private long _userId = -1;

        /// <summary>
        /// 当前的用户id，若用户未登录，则返回非正整数
        /// </summary>
        public long UserId => _userId <= 0 ? User.Identity.GetUserId<long>() : _userId;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName => User.Identity.GetUserName();

        /// <summary>
        /// Jti
        /// </summary>
        public string Jti => User.Identity.GetJti();
    }
}
