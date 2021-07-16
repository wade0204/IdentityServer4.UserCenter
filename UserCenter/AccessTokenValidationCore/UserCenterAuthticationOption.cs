using System;

namespace AccessTokenValidationCore
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UserCenterAuthticationOption
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
        /// 授权服务器地址
        /// </summary>
        public string Authority { get; set; }
    }
}
