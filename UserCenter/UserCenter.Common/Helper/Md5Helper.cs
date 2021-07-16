using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UserCenter.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class Md5Helper
    {
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Md5Encrypt32(this string password)
        {
            var cl = password;
            using MD5 md5 = MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            var s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            return s.Aggregate("", (current, t) => current + t.ToString("x2"));
        }
    }
}
