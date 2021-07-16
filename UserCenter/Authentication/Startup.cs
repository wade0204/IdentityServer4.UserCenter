using Authentication.Extensions;
using Authentication.Validator;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using UserCenter.Common;
using UserCenter.Repository;
using UserCenter.Service;

namespace Authentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);

            var freeSql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(FreeSql.DataType.MySql, appSettings.DbConfig.Connection)
                .UseAutoSyncStructure(true)
                .Build();
            services.AddSingleton(freeSql);

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

            RedisHelper.Initialization(new CSRedis.CSRedisClient(appSettings.RedisConfig.Connection));

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            containerBuilder.Build();
        }

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<ServicesModule>();
            builder.RegisterModule<RepositoryModule>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //初始化数据(内容后面描述)
            SeedData.InitData(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
            });

            app.UseAuthentication();
            app.UseIdentityServer();
        }
    }
}
