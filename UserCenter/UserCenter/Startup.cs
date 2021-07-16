using AccessTokenValidationCore;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserCenter.Common;
using UserCenter.Extensions;
using UserCenter.Repository;
using UserCenter.Service;

namespace UserCenter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            appSettings.Ids4Config.ClientSecret = Authentication.Config.defaltClientSecret;
            services.AddSingleton(appSettings);

            var sql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(FreeSql.DataType.MySql, appSettings.DbConfig.Connection)
                .UseAutoSyncStructure(true)
                .Build();
            services.AddSingleton(sql);

            services.AddControllers();

            services.AddSwaggerBuild();

            services.AddHttpClient();
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            services.AddHttpContextAccessor();

            RedisHelper.Initialization(new CSRedis.CSRedisClient(appSettings.RedisConfig.Connection));

            services.AddUserCenterBearerAuthentication(new UserCenterAuthticationOption
            {
                Authority = appSettings.Ids4Config.Authority,
                ClientScope = appSettings.Ids4Config.ClientScope,
                ClientId = appSettings.Ids4Config.ClientId,
                ClientSecret = appSettings.Ids4Config.ClientSecret 
            });

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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserCenter v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
            });

            app.AddUserCenterAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
