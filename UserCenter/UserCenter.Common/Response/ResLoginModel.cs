using System;
using UserCenter.Common.Entity;

namespace UserCenter.Common.Response
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ResLoginModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string scope { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserInfoEntity user { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string error { get; set; }
    }
}
