using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserCenter.IRepository
{
    public interface IBasicRepository<TEntity> : IReadOnlyRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 异步插入实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        Task<bool> InsertAsync(params TEntity[] entities);

        /// <summary>
        /// 异步删除实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        Task<bool> DeleteAsync(params TEntity[] entities);

        /// <summary>
        /// 异步更新实体对象
        /// </summary>
        /// <param name="entities">更新后的实体对象</param>
        /// <returns>操作影响的行数</returns>
        Task<bool> UpdateAsync(params TEntity[] entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateExpression"></param>
        /// <param name="ignoreColumns"></param>
        /// <returns></returns>
        Task<bool> UpdateBatchAsync(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, object>> ignoreColumns = null);
    }

    public interface IBasicRepository<TEntity, TKey> : IBasicRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>
        where TEntity : class
    {
    }

}
