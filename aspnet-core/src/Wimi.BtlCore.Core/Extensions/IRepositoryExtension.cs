namespace Wimi.BtlCore.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Abp.Domain.Entities;
    using Abp.Domain.Repositories;

    // ReSharper disable once InconsistentNaming
    public static class IRepositoryExtension
    {
        public static bool IsExist<TEntity, TKey>(
            this IRepository<TEntity, TKey> repository, 
            Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity<TKey>
        {
            return repository.GetAll().Any(expression);
        }

        public static async Task<bool> IsExistAsync<TEntity, TKey>(
            this IRepository<TEntity, TKey> repository, 
            Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity<TKey>
        {
            return await Task.FromResult(repository.IsExist(expression));
        }
    }
}