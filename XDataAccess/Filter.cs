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


public class Filter<T>
{
    private ParameterExpression Parameter { get; } = Expression.Parameter(typeof(T), "x");

    public Expression<Func<T, bool>> GenerateFilterExpression(FilterItem filter)
    {
        if (!filter.NestedFilters.Any()) return GenerateSimpleFilterExpression(filter);
        
        var nestedExpressions = filter.NestedFilters.Select(GenerateFilterExpression);
        return filter.Logic.ToLower() switch
        {
            "and" => CombineExpressions(nestedExpressions, Expression.AndAlso),
            "or" => CombineExpressions(nestedExpressions, Expression.OrElse),
            _ => throw new ArgumentException("Invalid logic operator")
        };
    }
    
    private Expression<Func<T, bool>> CombineExpressions(IEnumerable<Expression<Func<T, bool>>> expressions, Func<Expression, Expression, Expression> combiner)
    {
        Expression? combinedExpression = null;
        foreach (var expression in expressions)
        {
            combinedExpression = combinedExpression == null ? expression.Body : combiner(combinedExpression, expression.Body);
        }
        return Expression.Lambda<Func<T, bool>>(combinedExpression, Parameter);
    }

    private Expression<Func<T, bool>> GenerateSimpleFilterExpression(FilterItem filter)
    {
        var member = Expression.PropertyOrField(Parameter, filter.FieldName);
        var constant = Expression.Constant(filter.Value);
        return BuildComparisonExpression(member, filter.Operator, constant);
    }

    private Expression GetFilterExpression(FilterItem filter)
    {
        var member = GetNestedProperty(Parameter, filter.FieldName);
        var constant = Expression.Constant(filter.Value);
        return BuildComparisonExpression(member, filter.Operator, constant);
    }

    private Expression<Func<T, bool>> BuildComparisonExpression(Expression member, string op, ConstantExpression constant)
    {
        Expression operatorExpression;
        switch (op.ToLower())
        {
            case "eq":
                operatorExpression = Expression.Equal(member, constant);
                break;
            case "neq":
                operatorExpression = Expression.NotEqual(member, constant);
                break;
            case "gt":
                operatorExpression = Expression.GreaterThan(member, constant);
                break;
            case "lt":
                operatorExpression = Expression.LessThan(member, constant);
                break;
            case "gte":
                operatorExpression = Expression.GreaterThanOrEqual(member, constant);
                break;
            case "lte":
                operatorExpression = Expression.LessThanOrEqual(member, constant);
                break;
            case "contains":
                if (member.Type == typeof(string))
                {
                    operatorExpression = Expression.Call(member, "Contains", Type.EmptyTypes, constant);
                    break;
                }
                else
                {
                    operatorExpression = Expression.Call(typeof(Enumerable), "Contains", new[] { member.Type }, member, constant);
                    break;
                }
            case "doesnotcontain":
                if (member.Type == typeof(string))
                {
                    operatorExpression = Expression.Not(Expression.Call(member, "Contains", Type.EmptyTypes, constant));
                    break;
                }
                else
                {
                    operatorExpression = Expression.Not(Expression.Call(typeof(Enumerable), "Contains", new[] { member.Type }, member, constant));
                    break;
                }
            case "startswith":
                if (member.Type == typeof(string))
                {
                    operatorExpression = Expression.Call(member, "StartsWith", Type.EmptyTypes, constant);
                    break;
                }
                else
                {
                    throw new ArgumentException("Invalid operator for the specified type");
                }
            case "endswith":
                if (member.Type == typeof(string))
                {
                    operatorExpression = Expression.Call(member, "EndsWith", Type.EmptyTypes, constant);
                    break;
                }
                else
                {
                    throw new ArgumentException("Invalid operator for the specified type");
                }
            case "between":
                if (member.Type == typeof(DateTime))
                {
                    var constant2 = Expression.Constant((DateTime)constant.Value + TimeSpan.FromDays(1));
                    operatorExpression = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(member, constant),
                        Expression.LessThan(member, constant2));
                    break;
                }
                else if (IsNumericType(member.Type))
                {
                    var constant2 = Expression.Constant(Convert.ChangeType(constant.Value, member.Type));
                    operatorExpression = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(member, constant),
                        Expression.LessThanOrEqual(member, constant2));
                    break;
                }
                else
                {
                    throw new ArgumentException("Between operator is only supported for DateTime and numeric types.");
                }
            case "isnull":
                operatorExpression = Expression.Equal(member, Expression.Constant(null, member.Type));
                break;
            case "isnotnull":
                operatorExpression = Expression.NotEqual(member, Expression.Constant(null, member.Type));
                break;
            default:
                throw new ArgumentException("Invalid operator");
        }
        
        return Expression.Lambda<Func<T, bool>>(operatorExpression, Parameter);
    }

    private MemberExpression GetNestedProperty(Expression expression, string propertyName)
    {
        var properties = propertyName.Split('.');
        Expression result = expression;

        foreach (var prop in properties)
        {
            if (result.Type.IsGenericType && result.Type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = result.Type.GetGenericArguments()[0];
                result = Expression.Property(result, "Any", GetNestedProperty(Expression.Parameter(elementType, "e"), prop));
            }
            else
            {
                result = Expression.Property(result, prop);
            }
        }

        return (MemberExpression)result;
    }

    private bool IsNumericType(Type type)
    {
        return type == typeof(int) || type == typeof(double) || type == typeof(decimal) || type == typeof(float) ||
               type == typeof(long) || type == typeof(short) || type == typeof(uint) || type == typeof(ushort) ||
               type == typeof(ulong) || type == typeof(byte) || type == typeof(sbyte);
    }
}

public class FilterItem
{
    public string Logic { get; set; } // Logic operator for combining nested filters, e.g., AND, OR
    public List<FilterItem> NestedFilters { get; set; } // Nested filters
    public string FieldName { get; set; }
    public string Operator { get; set; }
    public object Value { get; set; }
}

