using System.Linq.Expressions;

namespace XDataAccess;

public interface IReadOnlyBasicRepository<TEntity> : IRepository
    where TEntity : Entity<TEntity>
{
    public Task<IList<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    
    public Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public DataSourceResult GetPagedListAsync(DataSourceRequest dataSourceRequest,
        CancellationToken cancellationToken = default);
}