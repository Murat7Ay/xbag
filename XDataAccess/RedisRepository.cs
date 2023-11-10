using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using Redis.OM;
using Redis.OM.Modeling;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace XDataAccess;

public class RedisRepository<TEntity> : IRepository<TEntity> where TEntity : Entity<TEntity>
{
    private readonly IDatabase _database;
    private readonly IRedisCollection<TEntity> _collection;
    private readonly IRedisCollection<EntityHistory> _historyCollection;
    private readonly IAuthUser _user;
    private readonly IClock _clock;
    private readonly IFilterCondition _filterCondition;
    private string EntityName => typeof(TEntity).Name;
    private RedisKey EntityXIdKey => $"{EntityName}:{nameof(IEntity<TEntity>.XId)}";

    private async Task<string> GetNextEntityXId()
    {
        long id = await _database.StringIncrementAsync(EntityXIdKey);
        string timestamp = _clock.Now.ToString("yyMMdd");
        return $"{timestamp}{id}";
    }

    public RedisRepository(
        RedisConnectionProvider provider,
        IDatabase database,
        IAuthUser user,
        IClock clock,
        IFilterCondition filterCondition)
    {
        _user = user;
        _clock = clock;
        _filterCondition = filterCondition;
        _collection = provider.RedisCollection<TEntity>();
        _historyCollection = provider.RedisCollection<EntityHistory>();
        _database = database;
    }

    private IRedisCollection<TEntity> ApplyFiltering(IRedisCollection<TEntity> query)
    {
        bool isDeletedFilter = _filterCondition.GetFilter("IsDeleted");
        bool isActiveFilter = _filterCondition.GetFilter("IsActive");
        return query.Where(x =>
            x.IsDeleted == isDeletedFilter &&
            x.IsActive == isActiveFilter);
    }

    public Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return filteredQuery
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

    public DataSourceResult GetPagedListAsync(DataSourceRequest dataSourceRequest,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return filteredQuery.ToDataSourceResult(dataSourceRequest);
    }

    public async Task<string> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(entity.Id))
        {
            throw new ArgumentException("Id must be null for new entities.", nameof(entity.Id));
        }

        await InitializeEntityForInsertAsync(entity);
        return await _collection.InsertAsync(entity);
    }

    private async Task InitializeEntityForInsertAsync(TEntity entity)
    {
        entity.Id = null;
        entity.XId = await GetNextEntityXId();
        entity.CreateDate = _clock.Now;
        entity.CreatedBy = _user.Id;
        entity.Ip = _user.Ip;
        entity.Host = _user.Host;
        entity.TraceId = _user.TraceId;
        entity.IsActive = true;
        entity.EntityVersion = 0;
        entity.ModifiedBy = null;
        entity.ModifyDate = null;
        entity.DeletedBy = null;
        entity.DeleteDate = null;
        entity.IsDeleted = false;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            throw new Exception("Id must be not null");
        }

        InitializeEntityForUpdate(entity);
        TEntity existingEntity = await CheckAndIncrementEntityVersion(entity);
        await SaveHistory(entity.GetChanges(existingEntity));
        await _collection.UpdateAsync(entity);
    }

    private void InitializeEntityForUpdate(TEntity entity)
    {
        entity.ModifiedBy = _user.Id;
        entity.ModifyDate = _clock.Now;
        entity.Ip = _user.Ip;
        entity.Host = _user.Host;
        entity.TraceId = _user.TraceId;
        entity.IsDeleted = false;
        entity.DeletedBy = null;
        entity.DeleteDate = null;
    }

    private async Task<TEntity> CheckAndIncrementEntityVersion(TEntity entity)
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

            if (existingEntity.XId != entity.XId)
            {
                throw new Exception("Entity has been modified");
            }

            entity.EntityVersion++;
            return existingEntity;
        }

        throw new ArgumentNullException(nameof(entity));
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TEntity existingEntity = await CheckAndMarkEntityAsDeleted(entity);
        await _collection.UpdateAsync(existingEntity);
    }

    private async Task<TEntity> CheckAndMarkEntityAsDeleted(TEntity entity)
    {
        if (entity.Id == null) throw new ArgumentNullException(nameof(entity));
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
        existingEntity.TraceId = _user.TraceId;
        existingEntity.Ip = _user.Ip;
        existingEntity.Host = _user.Host;
        existingEntity.TraceId = _user.TraceId;
        return existingEntity;
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = ApplyFiltering(_collection);
        return await filteredQuery.FirstOrDefaultAsync(predicate);
    }

    public async Task<TEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.FindByIdAsync(id);
    }

    public async Task<IList<EntityHistory>> GetHistoryAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _historyCollection.Where(x => x.EntityId == id).ToListAsync();
    }

    public IList<Dictionary<string, string>> GetIndexInfo()
    {
        //TODO: check multiple prefixes and none prefix on attribute
        string indexName = string.Join("",GetPrefixes()).ToLower(CultureInfo.InvariantCulture) + "-idx";
        RedisResult infoResult = _database.Execute("FT.INFO", indexName);

        if (infoResult.Type == ResultType.MultiBulk)
        {
            var infoPairs = (RedisResult[])infoResult;

            for (int i = 0; i < infoPairs.Length; i += 2)
            {
                string key = (string)infoPairs[i];
                RedisResult value = infoPairs[i + 1];

                switch (key)
                {
                    case "attributes":
                        return ParseIndexInfo(value);
                }
            }
        }

        return new List<Dictionary<string, string>>();
    }

    private IList<Dictionary<string, string>> ParseIndexInfo(RedisResult attributeResult)
    {
        if (attributeResult.Type != ResultType.MultiBulk)
        {
            return new List<Dictionary<string, string>>();
        }

        return ((RedisResult[])attributeResult)
            .Select(value => ((RedisResult[])value)
                .Where((_, index) => index % 2 == 0)
                .ToDictionary(key => key.ToString(), key => ((RedisResult[])value)[Array.IndexOf(((RedisResult[])value), key) + 1].ToString())
            )
            .ToList();
    }

    private async Task SaveHistory(IList<EntityChange> historyChanges)
    {
        if (historyChanges.Count == 0)
            return;
        var history = new EntityHistory();
        await _historyCollection.InsertAsync(history);
    }

    public IRedisCollection<TEntity> GetCollection => _collection;
    
    public string[] GetPrefixes()
    {
        Type entityType = typeof(TEntity);
        DocumentAttribute? documentAttribute = (DocumentAttribute)Attribute.GetCustomAttribute(entityType, typeof(DocumentAttribute));

        if (documentAttribute != null)
        {
            return documentAttribute.Prefixes;
        }

        return Array.Empty<string>();
    }
}