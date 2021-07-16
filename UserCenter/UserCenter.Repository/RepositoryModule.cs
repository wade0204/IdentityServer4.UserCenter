using Autofac;
using Autofac.AttributeExtensions;

namespace UserCenter.Repository
{
    /// <summary>
    /// 仓储模块
    /// </summary>
    public class RepositoryModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAttributedClasses(ThisAssembly);

            //builder.Register(context =>
            //{
            //    //扩展 可使用dapper
            //    var settings = context.Resolve<AppSettings>();
            //    return new MySqlConnection(settings.DbConfig.Connection);
            //}).AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope().Keyed<IDbConnection>(AutofacConst.DbConnection);
        }
    }
}
