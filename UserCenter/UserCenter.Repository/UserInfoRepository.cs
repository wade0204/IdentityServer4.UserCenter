using Autofac.AttributeExtensions;
using UserCenter.Common.Entity;
using UserCenter.IRepository;

namespace UserCenter.Repository
{
    [InstancePerLifetimeScope]
    public class UserInfoRepository : BaseRepository<UserInfoEntity, long>
    {
        public UserInfoRepository(IFreeSql fsql) : base(fsql)
        {
        }
    }
}
