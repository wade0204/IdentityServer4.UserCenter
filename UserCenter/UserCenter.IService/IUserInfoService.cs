using System.Threading.Tasks;
using UserCenter.Common.Entity;
using UserCenter.Common.Request;

namespace UserCenter.IService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserInfoEntity> Get(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UserInfoEntity> Login(ReqLoginModel model);
    }
}
