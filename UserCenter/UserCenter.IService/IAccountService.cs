using System.Threading.Tasks;
using UserCenter.Common.Request;
using UserCenter.Common.Response;

namespace UserCenter.IService
{
    /// <summary>
    /// 登录
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        Task<BaseResponse<ResLoginModel>> Login(ReqLoginModel loginModel);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse<bool>> LogOut(long userId, string jti);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<BaseResponse<ResLoginModel>> RefreshToken(string refreshToken);
    }
}
