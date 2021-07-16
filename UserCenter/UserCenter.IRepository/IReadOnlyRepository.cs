﻿using FreeSql;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserCenter.IRepository
{
    public interface IReadOnlyRepository<TEntity> : IRepository
        where TEntity : class
    {
        ISelect<TEntity> Select { get; }
    }

    public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity>
        where TEntity : class
    {
        TEntity Get(TKey id);
        Task<TEntity> GetAsync(TKey id);

        TEntity Find(TKey id);
        Task<TEntity> FindAsync(TKey id);
        Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
