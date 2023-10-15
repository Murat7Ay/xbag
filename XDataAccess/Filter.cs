using System.Linq.Expressions;
using System.Text.Json;

namespace XDataAccess;

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
        var propertyNames = Field.Split('.');
        Expression propertyAccess = parameter;
        foreach (var propertyName in propertyNames)
        {
            propertyAccess = Expression.Property(propertyAccess, propertyName);
        }
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