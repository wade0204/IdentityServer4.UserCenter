using FreeSql.DataAnnotations;
using System;
using UserCenter.Common.Enum;

namespace UserCenter.Common.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Table(Name = "UserJtiBlackList")]
    public class UserJtiBlackListEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true, DbType = "bigint", IsNullable = false)] 
        public long Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(DbType = "bigint", IsNullable = false)]
        public long UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(DbType = "varchar(30)", IsNullable = false, StringLength = 30)]
        public string Jti { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(DbType = "datetime", Name = "AddTime", IsNullable = false)]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(DbType = "tinyint", IsNullable = false, MapType = typeof(int))]
        public EnumStatus Status { get; set; }
    }
}
