using System.Text.Json.Serialization;

namespace XInfrastructure;

public class XValue : IXData
{
    public XType XType { get; }
    public object? Value { get; }

    [JsonConstructor]
    public XValue()
    {
    }

    public XValue(XType xType, object? value)
    {
        XType = xType;
        Value = value;
    }

    public static XValue Create(XType xType, object? value)
    {
        if (xType == XType.None || value == null)
        {
            return new XValue(xType, null);
        }

        if (!IsSupportedType(value.GetType()))
        {
            throw new ArgumentException("Unsupported value type for XValue");
        }

        return new XValue(xType, value);
    }

    private static bool IsSupportedType(Type type)
    {
        return type == typeof(long) ||
               type == typeof(int) ||
               type == typeof(short) ||
               type == typeof(string) ||
               type == typeof(bool) ||
               type == typeof(decimal) ||
               type == typeof(double) ||
               type == typeof(DateTime) ||
               type == typeof(XTable) ||
               type == typeof(XBag) ||
               type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    public override bool Equals(object? obj)
    {
        if (obj is XValue other)
        {
            return XType == other.XType && Equals(Value, other.Value);
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + XType.GetHashCode();
            hash = hash * 23 + (Value?.GetHashCode() ?? 0);
            return hash;
        }
    }
}