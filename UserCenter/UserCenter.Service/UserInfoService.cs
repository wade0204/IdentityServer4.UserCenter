using Autofac.AttributeExtensions;
using System;
using System.Threading.Tasks;
using UserCenter.Common.Const;
using UserCenter.Common.Entity;
using UserCenter.Common.Enum;
using UserCenter.Common.Request;
using UserCenter.IRepository;
using UserCenter.IService;

namespace UserCenter.Service
{
    /// <summary>
    /// 
    /// </summary>
    [InstancePerLifetimeScope]
    public class UserInfoService : IUserInfoService
    {
        private readonly IRepository<UserInfoEntity, long> _userInfoRepository;

        public UserInfoService(IRepository<UserInfoEntity, long> userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserInfoEntity> Get(long id)
        {
            return await _userInfoRepository.GetAsync(id);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserInfoEntity> Login(ReqLoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                return null;

            var key = $"{RedisKeyConst.Str_UserInfo}_{model.Username}";
            var res = await RedisHelper.GetAsync<UserInfoEntity>(key);
            if (res == null)
            {
                res = await _userInfoRepository.SelectAsync(x =>
                    x.UserName.Equals(model.Username) && x.Password.Equals(model.Password) &&
                    x.Status == EnumStatus.Normal);

                await RedisHelper.SetAsync(key, res, TimeSpan.FromMinutes(2));
            }

            return res;
        }
    }
}
