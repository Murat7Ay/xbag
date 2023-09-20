using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XInfrastructure;

[DebuggerDisplay("{DebuggerDisplay}")]
public class XValue : IXData
{
    public XType XType { get; }
    public object? Value { get; }

    [JsonConstructor]
    private XValue()
    {
    }

    private XValue(XType xType, object? value)
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

    public static implicit operator XValue(long value) => Create(XType.Long, value);

    public static implicit operator XValue(string value) => Create(XType.String, value);

    public static implicit operator XValue(bool value) => Create(XType.Bool, value);

    public static implicit operator XValue(decimal value) => Create(XType.Decimal, value);

    public static implicit operator XValue(double value) => Create(XType.Double, value);

    public static implicit operator XValue(DateTime value) => Create(XType.Date, value);

    public static implicit operator XValue(XBag value) => Create(XType.Bag, value);

    public static implicit operator XValue(List<long> value) => Create(XType.LongList, value);

    public static implicit operator XValue(List<string> value) => Create(XType.StringList, value);

    public static implicit operator XValue(List<bool> value) => Create(XType.BoolList, value);

    public static implicit operator XValue(List<decimal> value) => Create(XType.DecimalList, value);

    public static implicit operator XValue(List<double> value) => Create(XType.DoubleList, value);

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

    [DebuggerStepThrough]
    public override string ToString()
    {
        if (Value != null)
        {
            return $"{XType}: {Value}";
        }
        else
        {
            return $"{XType}: null";
        }
    }

    private string DebuggerDisplay
    {
        get
        {
            if (Value != null)
            {
                return $"{XType}: {JsonSerializer.Serialize(Value)}";
            }
            else
            {
                return $"{XType}: null";
            }
        }
    }
}