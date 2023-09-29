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

        // Convert the property access to the desired return type (TField)
        UnaryExpression propertyConversion = Expression.Convert(propertyAccess, typeof(TField));

        // Create the lambda expression (e.g., "x => x.PropertyName")
        Expression<Func<T, TField>> lambda = Expression.Lambda<Func<T, TField>>(propertyConversion, parameter);

        return lambda;
    }

    public static IRedisCollection<T> WhereIf<T>(this IRedisCollection<T> query, bool condition,
        Expression<Func<T, bool>> predicate) where T : IEntity<T>
    {
        Check.NotNull(query, nameof(query));

        return condition
            ? query.Where(predicate)
            : query;
    }

    public static IRedisCollection<T> OrderBySorting<T>(this IRedisCollection<T> query, string sorting) where T : IEntity<T>
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