using System.Linq.Expressions;

namespace XDataAccess;

public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IBasicRepository<TEntity>
    where TEntity : Entity
{
    public Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );
    public Task<TEntity?> FindByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );
}

public interface IRepository
{
}