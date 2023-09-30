using System.Linq.Expressions;

namespace XDataAccess;

public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IBasicRepository<TEntity>, IHistoryRepository<TEntity>
    where TEntity : Entity<TEntity>
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

public interface IHistoryRepository<TEntity> where TEntity : Entity<TEntity>
{
    public Task<IList<EntityHistory>> GetHistory(string id, CancellationToken cancellationToken = default);
    
}