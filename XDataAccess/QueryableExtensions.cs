using Redis.OM;
using Redis.OM.Searching;

namespace XDataAccess;

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
        return filter is null ? queryable : queryable.Where(filter.ToExpression<TEntity>());
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