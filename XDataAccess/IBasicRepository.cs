namespace XDataAccess;

public interface IBasicRepository<TEntity> : IReadOnlyBasicRepository<TEntity>
    where TEntity : IEntity<TEntity>
{
    public Task<string> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}