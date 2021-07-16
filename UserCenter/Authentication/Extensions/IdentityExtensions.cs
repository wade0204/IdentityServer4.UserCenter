using System;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace Authentication.Extensions
{
    /// <summary>
    /// Identity 相关扩展
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        ///     Return the claim value for the first claim with the specified type if it exists, null otherwise
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            var claim = identity.FindFirst(claimType);
            return claim?.Value;
        }

        /// <summary>
        ///     Return the user id using the UserIdClaimType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static T GetUserId<T>(this IIdentity identity)
            where T : IConvertible
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            var claimsIdentity = identity as ClaimsIdentity;
            var str = claimsIdentity?.FindFirstValue(
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (str != null)
            {
                return (T)Convert.ChangeType(str, typeof(T), CultureInfo.InvariantCulture);
            }
            return default(T);
        }

        /// <summary>
        ///     Return the user name using the UserNameClaimType
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetUserName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        }

        /// <summary>
        ///     Return the user name using the JtiClaimType
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetJti(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity?.FindFirstValue("jti");
        }
    }
}
