using FreeSql.DataAnnotations;
using UserCenter.Common.Enum;

namespace UserCenter.Common.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Table(Name = "UserInfo")]
    public class UserInfoEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true, DbType = "bigint", IsNullable = false)] 
        public long Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(DbType = "varchar(20)", IsNullable = false, StringLength = 20)]
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(DbType = "varchar(32)", IsNullable = false, StringLength = 32)]
        public string Password { get; set; }

        [Column(DbType = "tinyint", IsNullable = false, MapType = typeof(int))]
        public EnumStatus Status { get; set; }
    }
}
