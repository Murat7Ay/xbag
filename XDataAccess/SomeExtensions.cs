using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
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

public static class QueryableExtensions
{
    public static DataSourceResult ToDataSourceResult<TEntity>(this IRedisCollection<TEntity> queryable, int take,
        int skip,
        List<Sort>? sort, Filter? filter, IEnumerable<Aggregator>? aggregates) where TEntity : Entity<TEntity>
    {
        // Filter the data first
        queryable = Filter(queryable, filter);

        // Calculate the total number of records (needed for paging)
        var total = queryable.Count();

        // Calculate the aggregates
        var aggregate = Aggregate(queryable, aggregates);

        // Sort the data
        queryable = Sort(queryable, sort);

        // Finally page the data
        if (take > 0)
        {
            queryable = Page(queryable, take, skip);
        }

        return new DataSourceResult
        {
            Data = queryable.ToList(),
            Total = total,
            Aggregates = aggregate
        };
    }

    public static DataSourceResult ToDataSourceResult<TEntity>(this IRedisCollection<TEntity> queryable, int take,
        int skip,
        List<Sort>? sort, Filter? filter) where TEntity : Entity<TEntity>
    {
        return queryable.ToDataSourceResult(take, skip, sort, filter, null);
    }

    public static DataSourceResult ToDataSourceResult<TEntity>(this IRedisCollection<TEntity> queryable,
        DataSourceRequest request) where TEntity : Entity<TEntity>
    {
        return queryable.ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter, null);
    }

    private static IRedisCollection<TEntity> Filter<TEntity>(IRedisCollection<TEntity> queryable, Filter? filter)
        where TEntity : Entity<TEntity>
    {
        if (filter is not { Logic: { } }) return queryable;
        return queryable.Where(filter.ToExpression<TEntity>());
    }

    private static object Aggregate<TEntity>(IRedisCollection<TEntity> queryable, IEnumerable<Aggregator>? aggregates)
    {
        return null;
        // if (aggregates != null && aggregates.Any())
        // {
        //     var objProps = new Dictionary<DynamicProperty, object>();
        //     var groups = aggregates.GroupBy(g => g.Field);
        //     Type type = null;
        //     foreach (var group in groups)
        //     {
        //         var fieldProps = new Dictionary<DynamicProperty, object>();
        //         foreach (var aggregate in group)
        //         {
        //             var prop = typeof(TEntity).GetProperty(aggregate.Field);
        //             var param = Expression.Parameter(typeof(TEntity), "s");
        //             var selector = aggregate.Aggregate == "count" &&
        //                            (Nullable.GetUnderlyingType(prop.PropertyType) != null)
        //                 ? Expression.Lambda(
        //                     Expression.NotEqual(Expression.MakeMemberAccess(param, prop),
        //                         Expression.Constant(null, prop.PropertyType)), param)
        //                 : Expression.Lambda(Expression.MakeMemberAccess(param, prop), param);
        //             var mi = aggregate.MethodInfo(typeof(TEntity));
        //             if (mi == null)
        //                 continue;
        //
        //             var val = queryable.Provider.Execute(Expression.Call(null, mi,
        //                 aggregate.Aggregate == "count" && (Nullable.GetUnderlyingType(prop.PropertyType) == null)
        //                     ? new[] { queryable.Expression }
        //                     : new[] { queryable.Expression, Expression.Quote(selector) }));
        //
        //             fieldProps.Add(new DynamicProperty(aggregate.Aggregate, typeof(object)), val);
        //         }
        //
        //         type = DynamicExpression.CreateClass(fieldProps.Keys);
        //         var fieldObj = Activator.CreateInstance(type);
        //         foreach (var p in fieldProps.Keys)
        //             type.GetProperty(p.Name).SetValue(fieldObj, fieldProps[p], null);
        //         objProps.Add(new DynamicProperty(group.Key, fieldObj.GetType()), fieldObj);
        //     }
        //
        //     type = DynamicExpression.CreateClass(objProps.Keys);
        //
        //     var obj = Activator.CreateInstance(type);
        //
        //     foreach (var p in objProps.Keys)
        //     {
        //         type.GetProperty(p.Name).SetValue(obj, objProps[p], null);
        //     }
        //
        //     return obj;
        // }
        // else
        // {
        //     return null;
        // }
    }

    private static IRedisCollection<TEntity> Sort<TEntity>(IRedisCollection<TEntity> queryable, List<Sort>? sort)
        where TEntity : Entity<TEntity>
    {
        if (sort != null && sort.Any())
        {
            return queryable.OrderBySorting(sort);
        }

        return queryable;
    }

    private static IRedisCollection<TEntity> Page<TEntity>(IRedisCollection<TEntity> queryable, int take, int skip)
        where TEntity : Entity<TEntity>
    {
        return queryable.Skip(skip).Take(take);
    }
}

public class Sort
{
    public string Field { get; set; }

    public string Dir { get; set; }

    public string ToExpression()
    {
        return Field + " " + Dir;
    }
}

public class Filter
{
    public string Field { get; set; }

    public string Operator { get; set; }

    public object Value { get; set; }

    public string Logic { get; set; }

    public IEnumerable<Filter>? Filters { get; set; }

    private static readonly IDictionary<string, string> operators = new Dictionary<string, string>
    {
        { "eq", "=" },
        { "neq", "!=" },
        { "lt", "<" },
        { "lte", "<=" },
        { "gt", ">" },
        { "gte", ">=" },
        { "startswith", "StartsWith" },
        { "endswith", "EndsWith" },
        { "contains", "Contains" },
        { "doesnotcontain", "Contains" }
    };

    public IList<Filter> All()
    {
        var filters = new List<Filter>();

        Collect(filters);

        return filters;
    }

    private void Collect(IList<Filter> filters)
    {
        if (Filters != null && Filters.Any())
        {
            foreach (Filter filter in Filters)
            {
                filters.Add(filter);

                filter.Collect(filters);
            }
        }
        else
        {
            filters.Add(this);
        }
    }

    public Expression<Func<T, bool>> ToExpression<T>()
    {
        var parameter = Expression.Parameter(typeof(T), "item");

        // Build the initial expression based on the first filter
        var expression = BuildExpression<T>(parameter);

        // Apply logical operators for additional filters
        if (Filters != null && Filters.Any())
        {
            foreach (var filter in Filters)
            {
                Expression<Func<T, bool>> filterExpression = filter.BuildExpression<T>(parameter);

                if (filter.Logic == "and")
                {
                    expression =
                        Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, filterExpression.Body),
                            parameter);
                }
                else if (filter.Logic == "or")
                {
                    expression =
                        Expression.Lambda<Func<T, bool>>(Expression.OrElse(expression.Body, filterExpression.Body),
                            parameter);
                }
            }
        }

        return expression;
    }

    private Expression<Func<T, bool>> BuildExpression<T>(ParameterExpression parameter)
    {
        Expression propertyAccess = Expression.Property(parameter, Field);
        Expression constant;

        // Convert Jsonelement to constant value
        if (Value is JsonElement jsonElement)
        {
            constant = ConvertJsonElementToConstant(jsonElement, propertyAccess.Type);
        }
        else
        {
            constant = Expression.Constant(Value);
        }

        Expression operatorExpression = Expression.Equal(propertyAccess, constant);

        if (operators.TryGetValue(Operator, out string mappedOperator))
        {
            switch (mappedOperator)
            {
                case "=":
                    operatorExpression = Expression.Equal(propertyAccess, constant);
                    break;
                case "!=":
                    operatorExpression = Expression.NotEqual(propertyAccess, constant);
                    break;
                case "<":
                    operatorExpression = Expression.LessThan(propertyAccess, constant);
                    break;
                case "<=":
                    operatorExpression = Expression.LessThanOrEqual(propertyAccess, constant);
                    break;
                case ">":
                    operatorExpression = Expression.GreaterThan(propertyAccess, constant);
                    break;
                case ">=":
                    operatorExpression = Expression.GreaterThanOrEqual(propertyAccess, constant);
                    break;
                case "StartsWith":
                    operatorExpression = Expression.Call(propertyAccess,
                        typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant);
                    break;
                case "EndsWith":
                    operatorExpression = Expression.Call(propertyAccess,
                        typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant);
                    break;
                case "Contains":
                    operatorExpression = Expression.Call(propertyAccess,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant);
                    break;
                case "DoesNotContain":
                    if (propertyAccess.Type == typeof(string))
                    {
                        // Use the String.Contains method and negate the result
                        operatorExpression = Expression.Not(
                            Expression.Call(propertyAccess,
                                typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant)
                        );
                    }

                    break;
                case "Between":
                    if (Value is List<object> rangeValues && rangeValues.Count == 2)
                    {
                        Expression lowerBound = Expression.Constant(rangeValues[0]);
                        Expression upperBound = Expression.Constant(rangeValues[1]);

                        // Create a binary expression to check if the value is between the lower and upper bounds
                        operatorExpression = Expression.AndAlso(
                            Expression.GreaterThanOrEqual(propertyAccess, lowerBound),
                            Expression.LessThanOrEqual(propertyAccess, upperBound)
                        );
                    }

                    break;
            }
        }

        return Expression.Lambda<Func<T, bool>>(operatorExpression, parameter);
    }

    private Expression ConvertJsonElementToConstant(JsonElement jsonElement, Type targetType)
    {
        // Handle different types of JsonElement as needed
        if (jsonElement.ValueKind == JsonValueKind.String)
        {
            if (targetType == typeof(string))
            {
                return Expression.Constant(jsonElement.GetString());
            }
            else if (targetType == typeof(int))
            {
                if (int.TryParse(jsonElement.GetString(), out int intValue))
                {
                    return Expression.Constant(intValue);
                }
            }
            else if (targetType == typeof(long))
            {
                if (long.TryParse(jsonElement.GetString(), out long longValue))
                {
                    return Expression.Constant(longValue);
                }
            }
            // Handle other type conversions as needed
        }
        else if (jsonElement.ValueKind == JsonValueKind.Number)
        {
            if (targetType == typeof(int))
            {
                if (jsonElement.TryGetInt32(out int intValue))
                {
                    return Expression.Constant(intValue);
                }
            }
            else if (targetType == typeof(long))
            {
                if (jsonElement.TryGetInt64(out long longValue))
                {
                    return Expression.Constant(longValue);
                }
            }
            else if (targetType == typeof(double))
            {
                if (jsonElement.TryGetDouble(out double doubleValue))
                {
                    return Expression.Constant(doubleValue);
                }
            }
            else if (targetType == typeof(string))
            {
                return Expression.Constant(jsonElement.ToString());
            }
            // Handle other type conversions as needed
        }
        // Handle other JsonValueKind cases as needed

        // If the conversion fails, you can return a default constant or throw an exception.
        throw new NotSupportedException($"Unsupported JsonElement value kind: {jsonElement.ValueKind}");
    }
}

public class Aggregator
{
    public string Field { get; set; }

    public string Aggregate { get; set; }

    public MethodInfo MethodInfo(Type type)
    {
        var proptype = type.GetProperty(Field).PropertyType;
        switch (Aggregate)
        {
            case "max":
            case "min":
                return GetMethod(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Aggregate), MinMaxFunc().Method, 2)
                    .MakeGenericMethod(type, proptype);
            case "average":
            case "sum":
                return GetMethod(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Aggregate),
                    ((Func<Type, Type[]>)this.GetType()
                        .GetMethod("SumAvgFunc", BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(proptype).Invoke(null, null)).Method, 1).MakeGenericMethod(type);
            case "count":
                return GetMethod(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Aggregate),
                        Nullable.GetUnderlyingType(proptype) != null ? CountNullableFunc().Method : CountFunc().Method,
                        1)
                    .MakeGenericMethod(type);
        }

        return null;
    }

    private static MethodInfo GetMethod(string methodName, MethodInfo methodTypes, int genericArgumentsCount)
    {
        var methods = from method in typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            let parameters = method.GetParameters()
            let genericArguments = method.GetGenericArguments()
            where method.Name == methodName &&
                  genericArguments.Length == genericArgumentsCount &&
                  parameters.Select(p => p.ParameterType)
                      .SequenceEqual((Type[])methodTypes.Invoke(null, genericArguments))
            select method;
        return methods.FirstOrDefault();
    }

    private static Func<Type, Type[]> CountNullableFunc()
    {
        return (T) => new[]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(T, typeof(bool)))
        };
    }

    private static Func<Type, Type[]> CountFunc()
    {
        return (T) => new[]
        {
            typeof(IQueryable<>).MakeGenericType(T)
        };
    }

    private static Func<Type, Type, Type[]> MinMaxFunc()
    {
        return (T, U) => new[]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(T, U))
        };
    }

    private static Func<Type, Type[]> SumAvgFunc<U>()
    {
        return (T) => new[]
        {
            typeof(IQueryable<>).MakeGenericType(T),
            typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(T, typeof(U)))
        };
    }
}

public class DataSourceResult
{
    public IEnumerable Data { get; set; }

    public int Total { get; set; }

    public object Aggregates { get; set; }

    private static Type[] GetKnownTypes()
    {
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.FullName.StartsWith("DynamicClasses"));

        if (assembly == null)
        {
            return new Type[0];
        }

        return assembly.GetTypes()
            .Where(t => t.Name.StartsWith("DynamicClass"))
            .ToArray();
    }
}

public class DataSourceRequest
{
    public int Take { get; set; }
    public int Skip { get; set; }
    public List<Sort>? Sort { get; set; }
    public Filter? Filter { get; set; }
}