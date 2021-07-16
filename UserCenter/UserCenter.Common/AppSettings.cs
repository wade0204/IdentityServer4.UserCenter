namespace UserCenter.Common
{
    /// <summary>
    /// 公共配置文件
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConfig DbConfig { get; set; }

        /// <summary>
        /// redis连接
        /// </summary>
        public RedisConfig RedisConfig { get; set; }

        /// <summary>
        /// ids4配置文件
        /// </summary>
        public Ids4Config Ids4Config { get; set; }
    }

    /// <summary>
    /// 数据库连接
    /// </summary>
    public class DbConfig
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public string Ids4ConfigConnection { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public string Ids4OpConnection { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public string Ids4Connection { get; set; }
    }

    /// <summary>
    /// redis连接
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// redis连接
        /// </summary>
        public string Connection { get; set; }
    }

    /// <summary>
    /// ids4配置文件
    /// </summary>
    public class Ids4Config
    {
        /// <summary>
        /// 客户端id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端密钥
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 客户端范围
        /// </summary>
        public string ClientScope { get; set; }

        /// <summary>
        /// 授予类型
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// 授予类型:刷新令牌
        /// </summary>
        public string GrantTypeRefreshToken { get; set; }

        /// <summary>
        /// 授权服务器地址
        /// </summary>
        public string Authority { get; set; }
    }
}
