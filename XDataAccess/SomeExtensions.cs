using System.Linq.Expressions;
using Redis.OM;
using Redis.OM.Searching;

namespace XDataAccess;

public static class SomeExtensions
{
    private static Expression<Func<T, TField>> CreateExpression<T, TField>(string propertyName)
    {
        // Input parameter (e.g., "x")
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        // Property access (e.g., "x.PropertyName")
        MemberExpression propertyAccess = Expression.Property(parameter, propertyName);

        // Create the lambda expression (e.g., "x => x.PropertyName")
        Expression<Func<T, TField>> lambda = Expression.Lambda<Func<T, TField>>(propertyAccess, parameter);

        return lambda;
    }

    public static IRedisCollection<TEntity> WhereIf<TEntity>(this IRedisCollection<TEntity> query, bool condition,
        Expression<Func<TEntity, bool>> predicate) where TEntity : Entity<TEntity>
    {
        Check.NotNull(query, nameof(query));

        return condition ? query.Where(predicate) : query;
    }

    public static IRedisCollection<TEntity> OrderBySorting<TEntity>(this IRedisCollection<TEntity> query,
        List<Sort>? sort) where TEntity : Entity<TEntity>
    {
        if (sort == null) return query;
        foreach (var sortingItem in sort)
        {
            if (!Enum.TryParse(sortingItem.Dir, true, out SortDirection sortDirection))
            {
                continue;
            }

            var expression = CreateExpression<TEntity, string>(sortingItem.Field);
            query = sortDirection switch
            {
                SortDirection.Asc => query.OrderBy(expression),
                SortDirection.Desc => query.OrderByDescending(expression),
                _ => query
            };
        }

        return query;
    }
}