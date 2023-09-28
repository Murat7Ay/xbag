using System.Linq.Expressions;
using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace XDataAccess;

public class RedisRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly IDatabase _database;
    private readonly RedisCollection<TEntity> _collection;
    private readonly SessionUser _user;
    private readonly IClock _clock;
    private readonly IFilterCondition _filterCondition;

    public RedisRepository(
        ConfigurationOptions configurationOptions,
        SessionUser user,
        IClock clock,
        IFilterCondition filterCondition)
    {
        _user = user;
        _clock = clock;
        _filterCondition = filterCondition;
        var provider = new RedisConnectionProvider(configurationOptions);
        _collection = (RedisCollection<TEntity>)provider.RedisCollection<TEntity>();
        IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        _database = multiplexer.GetDatabase();
    }

    private IRedisCollection<TEntity> ApplyFiltering(IRedisCollection<TEntity> query)
    {
        var filteredQuery = query
            .WhereIf(_filterCondition.GetFilter("IsDeleted"), x => !x.IsDeleted)
            .WhereIf(_filterCondition.GetFilter("IsActive"), x => x.IsActive);

        return filteredQuery;
    }

    public async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return await filteredQuery
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<IList<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return await filteredQuery.ToListAsync();
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return await filteredQuery.CountAsync();
    }

    public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return await filteredQuery.CountAsync(predicate);
    }

    public async Task<IList<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        var orderedQuery = filteredQuery.OrderBySorting(sorting);
        return await orderedQuery
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }

    public async Task<string> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(entity.Id))
        {
            throw new Exception("Id must be null");
        }

        InitializeEntityForInsert(entity);
        return await _collection.InsertAsync(entity);
    }

    private void InitializeEntityForInsert(TEntity entity)
    {
        entity.CreateDate = _clock.Now;
        entity.CreatedBy = _user.Id;
        entity.Ip = _user.Ip;
        entity.Host = _user.Host;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            throw new Exception("Id must be not null");
        }

        InitializeEntityForUpdate(entity);
        await CheckAndIncrementEntityVersion(entity);
        await _collection.UpdateAsync(entity);
    }

    private void InitializeEntityForUpdate(TEntity entity)
    {
        entity.ModifiedBy = _user.Id;
        entity.ModifyDate = _clock.Now;
        entity.Ip = _user.Ip;
        entity.Host = _user.Host;
    }

    private async Task CheckAndIncrementEntityVersion(TEntity entity)
    {
        if (entity.Id != null)
        {
            TEntity? existingEntity = await _collection.FindByIdAsync(entity.Id);

            if (existingEntity is null)
            {
                throw new Exception("Entity is null");
            }

            if (existingEntity.EntityVersion != entity.EntityVersion)
            {
                throw new Exception("Entity has been modified");
            }
            
            entity.EntityVersion++;
        }
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        InitializeEntityForUpdate(entity);
        await CheckAndMarkEntityAsDeleted(entity);
        await _collection.UpdateAsync(entity);
    }

    private async Task CheckAndMarkEntityAsDeleted(TEntity entity)
    {
        if (entity.Id != null)
        {
            TEntity? existingEntity = await _collection.FindByIdAsync(entity.Id);

            if (existingEntity is null)
            {
                throw new Exception("Entity is null");
            }

            if (existingEntity.EntityVersion != entity.EntityVersion)
            {
                throw new Exception("Entity has been modified");
            }

            existingEntity.DeletedBy = _user.Id;
            existingEntity.DeleteDate = _clock.Now;
            existingEntity.IsDeleted = true;
        }
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return await filteredQuery.FirstOrDefaultAsync(predicate);
    }

    public async Task<TEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return await filteredQuery.FindByIdAsync(id);
    }
}