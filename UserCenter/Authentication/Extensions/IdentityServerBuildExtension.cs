using Authentication.Validator;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;
using UserCenter.Common;

namespace Authentication.Extensions
{
    internal static class IdentityServerBuildExtension
    {
        internal static IIdentityServerBuilder AddIdentityServerBuild(this IServiceCollection services, AppSettings appSettings)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var ver = new MySqlServerVersion(new Version(8, 0, 25));

            var builders = services.AddIdentityServer(
                    options =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                    })
                //添加这配置数据(客户端、资源)
                .AddConfigurationStore(options => //添加配置数据（ConfigurationDbContext上下文用户配置数据）
                {
                    options.ConfigureDbContext = option => option.UseMySql(
                        appSettings.DbConfig.Ids4ConfigConnection, ver,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                //添加操作数据(codes、tokens、consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = option => option.UseMySql(
                        appSettings.DbConfig.Ids4OpConnection, ver, sql => sql.MigrationsAssembly(migrationsAssembly));
                    //token自动清理
                    options.EnableTokenCleanup = true;
                    ////token自动清理间隔：默认1H
                    options.TokenCleanupInterval = 60 * 60 * 24;
                })
                .AddDeveloperSigningCredential();
            //.AddInMemoryIdentityResources(Config.GetIdentityResources())
            //.AddInMemoryApiResources(Config.GetApiResources())
            //.AddInMemoryClients(Config.GetClients());
            builders.Services.RemoveAll<ITokenService>();
            builders.Services.TryAddTransient<ITokenService, CustomTokenService>();
            builders.Services.AddTransient<IProfileService, CustomProfileService>();
            builders.Services.AddTransient<IProfileService, ProfileService>();
            builders.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
            builders.AddExtensionGrantValidator<AnonymousGrantValidator>();

            return builders;
        }
    }
}
