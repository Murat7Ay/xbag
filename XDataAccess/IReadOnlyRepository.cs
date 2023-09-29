using System.Linq.Expressions;

namespace XDataAccess;

public interface IReadOnlyRepository<TEntity> : IReadOnlyBasicRepository<TEntity>
    where TEntity : IEntity<TEntity>
{
    public Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}