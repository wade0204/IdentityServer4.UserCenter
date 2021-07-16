using FreeSql;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserCenter.IRepository
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected IFreeSql _fsql;

        protected BaseRepository(IFreeSql fsql) : base()
        {
            _fsql = fsql;
            if (_fsql == null) throw new NullReferenceException("fsql 参数不可为空");
        }

        public ISelect<TEntity> Select => _fsql.Select<TEntity>();

        /// <summary>
        /// 异步删除实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public async Task<bool> DeleteAsync(params TEntity[] entities) => await _fsql.Delete<TEntity>(entities).ExecuteAffrowsAsync() > 0;

        /// <summary>
        /// 异步插入实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public async Task<bool> InsertAsync(params TEntity[] entities) => await _fsql.Insert(entities).ExecuteAffrowsAsync() > 0;

        /// <summary>
        /// 异步更新实体对象
        /// </summary>
        /// <param name="entities">更新后的实体对象</param>
        /// <returns>操作影响的行数</returns>
        public async Task<bool> UpdateAsync(params TEntity[] entities) => await _fsql.Update<TEntity>().SetSource(entities).ExecuteAffrowsAsync() > 0;

        /// <summary>
        /// 异步更新实体对象
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateExpression"></param>
        /// <param name="ignoreColumns"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBatchAsync(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, object>> ignoreColumns = null)
        {
            var freeSqlHandle = _fsql.Update<TEntity>().Set(updateExpression);

            if (ignoreColumns != null)
                freeSqlHandle.IgnoreColumns(ignoreColumns);
            
            return await freeSqlHandle.Where(predicate).ExecuteAffrowsAsync() > 0;
        }
    }

    public abstract class BaseRepository<TEntity, TKey> : BaseRepository<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class
    {
        protected BaseRepository(IFreeSql fsql) : base(fsql)
        {
        }

        public TEntity Find(TKey id) => _fsql.Select<TEntity>(id).ToOne();
        public Task<TEntity> FindAsync(TKey id) => _fsql.Select<TEntity>(id).ToOneAsync();

        public TEntity Get(TKey id) => Find(id);
        public Task<TEntity> GetAsync(TKey id) => FindAsync(id);

        public Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate) => _fsql.Select<TEntity>().Where(predicate).ToOneAsync();
    }
}
