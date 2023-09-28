using System.IO.Compression;
using System.Linq.Expressions;
using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace XDataAccess;

public class RedisRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private RedisConnectionProvider _provider;
    private IDatabase _database;
    private readonly RedisCollection<TEntity> _collection;
    private SessionUser _user;
    private readonly IClock _clock;
    private readonly IFilterCondition _filterCondition;

    public RedisRepository(ConfigurationOptions configurationOptions, SessionUser user, IClock clock,
        IFilterCondition filterCondition)
    {
        _user = user;
        _clock = clock;
        _filterCondition = filterCondition;
        _provider = new RedisConnectionProvider(configurationOptions);
        _collection = (RedisCollection<TEntity>)_provider.RedisCollection<TEntity>();
        IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        _database = multiplexer.GetDatabase();
    }

    public Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _collection
            .WhereIf(_filterCondition.GetFilter("IsDeleted"), x => x.IsDeleted == false)
            .WhereIf(_filterCondition.GetFilter("IsActive"), x => x.IsActive == true)
            .Where(predicate)
            .ToListAsync();
    }

    public Task<IList<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return _collection
            .WhereIf(_filterCondition.GetFilter("IsDeleted"), x => x.IsDeleted == false)
            .WhereIf(_filterCondition.GetFilter("IsActive"), x => x.IsActive == true)
            .ToListAsync();
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return _collection
            .WhereIf(_filterCondition.GetFilter("IsDeleted"), x => x.IsDeleted == false)
            .WhereIf(_filterCondition.GetFilter("IsActive"), x => x.IsActive == true)
            .CountAsync();
    }

    public Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _collection
            .WhereIf(_filterCondition.GetFilter("IsDeleted"), x => x.IsDeleted == false)
            .WhereIf(_filterCondition.GetFilter("IsActive"), x => x.IsActive == true)
            .CountAsync(predicate);
    }

    public Task<IList<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting,
        CancellationToken cancellationToken = default)
    {
        return _collection
            .WhereIf(_filterCondition.GetFilter("IsDeleted"), x => x.IsDeleted == false)
            .WhereIf(_filterCondition.GetFilter("IsActive"), x => x.IsActive == true)
            .OrderBySorting(sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }

    public Task<string> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(entity.Id))
        {
            throw new Exception("Id must be null");
        }

        entity.CreateDate = _clock.Now;
        entity.CreatedBy = _user.Id;
        entity.Ip = _user.Ip;
        entity.Host = _user.Host;
        return _collection.InsertAsync(entity);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(entity.Id))
            throw new Exception("Id must be not null");
        CheckEntityVersion(entity);
        entity.EntityVersion++;
        entity.ModifiedBy = _user.Id;
        entity.ModifyDate = _clock.Now;
        entity.Ip = _user.Ip;
        entity.Host = _user.Host;
        return _collection.UpdateAsync(entity);
    }

    private TEntity CheckEntityVersion(TEntity entity)
    {
        TEntity? existingEntity = _collection.FindById(entity.Id!);
        if (existingEntity is null)
            throw new Exception("Entity is null");
        if (existingEntity.EntityVersion != entity.EntityVersion)
            throw new Exception("Entity has been modified");
        return existingEntity;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TEntity existingEntity = CheckEntityVersion(entity);
        existingEntity.DeletedBy = _user.Id;
        existingEntity.DeleteDate = _clock.Now;
        existingEntity.IsDeleted = true;
        existingEntity.Ip = _user.Ip;
        existingEntity.Host = _user.Host;
        return _collection.UpdateAsync(existingEntity);
    }

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _collection
            .WhereIf(_filterCondition.GetFilter("IsDeleted"), x => x.IsDeleted == false)
            .WhereIf(_filterCondition.GetFilter("IsActive"), x => x.IsActive == true)
            .FirstOrDefaultAsync(predicate);
    }

    public Task<TEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection
            .WhereIf(_filterCondition.GetFilter("IsDeleted"), x => x.IsDeleted == false)
            .WhereIf(_filterCondition.GetFilter("IsActive"), x => x.IsActive == true)
            .FindByIdAsync(id);
    }
}

public class SessionUser
{
    public string? Id { get; set; }
    public string? Ip { get; set; }
    public string? Host { get; set; }
}

public interface IFilterCondition
{
    public bool GetFilter(string key);
}

public interface IClock
{
    public DateTime Now { get; }
    public DateTime ProcessDate { get; }
}

public static class SomeExtensions
{
    private static Expression<Func<T, TField>> CreateExpression<T, TField>(string propertyName)
    {
        // Input parameter (e.g., "x")
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        // Property access (e.g., "x.PropertyName")
        MemberExpression propertyAccess = Expression.Property(parameter, propertyName);

        // Convert the property access to the desired return type (TField)
        UnaryExpression propertyConversion = Expression.Convert(propertyAccess, typeof(TField));

        // Create the lambda expression (e.g., "x => x.PropertyName")
        Expression<Func<T, TField>> lambda = Expression.Lambda<Func<T, TField>>(propertyConversion, parameter);

        return lambda;
    }

    public static IRedisCollection<T> WhereIf<T>(this IRedisCollection<T> query, bool condition,
        Expression<Func<T, bool>> predicate) where T : Entity
    {
        Check.NotNull(query, nameof(query));

        return condition
            ? query.Where(predicate)
            : query;
    }

    public static IRedisCollection<T> OrderBySorting<T>(this IRedisCollection<T> query, string sorting) where T : Entity
    {
        var sortingItems = sorting.Split(',').Select(item => item.Trim());

        foreach (var sortingItem in sortingItems)
        {
            var parts = sortingItem.Split(' ');
            if (parts.Length != 2)
            {
                // Handle invalid input if needed
                continue;
            }

            var propertyName = parts[0];
            var sortOrder = parts[1].ToUpper(); // Convert to uppercase for case-insensitive parsing

            if (!Enum.TryParse(sortOrder, true, out SortDirection sortDirection))
            {
                // Handle invalid sorting direction if needed
                continue;
            }

            var expression = CreateExpression<T, string>(propertyName);

            if (sortDirection == SortDirection.Asc)
            {
                query.OrderBy(expression);
            }
            else if (sortDirection == SortDirection.Desc)
            {
                query.OrderByDescending(expression);
            }
        }

        return query;
    }
}

public enum SortDirection
{
    Asc,
    Desc
}

public static class Check
{
    public static T NotNull<T>(
        T? value,
        string parameterName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }
}