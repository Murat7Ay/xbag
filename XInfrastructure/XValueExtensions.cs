using System.Runtime.CompilerServices;

namespace XInfrastructure;

public static class XValueExtensions
{
    public static string To_String(this XValue value)
    {
        if (value.XType == XType.None || value.Value is null)
        {
            return "";
        }

        switch (value.XType)
        {
            case XType.Int:
            case XType.Long:
            {
                if (long.TryParse(value.Value.ToString(), out var number))
                {
                    return number.ToString();
                }

                break;
            }
            case XType.Bool when bool.TryParse(value.Value.ToString(), out var boolValue):
                return boolValue.ToString();
            case XType.Decimal when decimal.TryParse(value.Value.ToString(), out var decimalValue):
                return decimalValue.ToString("0.##########"); // Format with 10 digits after the decimal point
            case XType.Double when double.TryParse(value.Value.ToString(), out var doubleValue):
                return doubleValue.ToString("0.##########"); // Format with 10 digits after the decimal point
        }

        return "";
    }

    public static long To_Long(this XValue value)
    {
        if (value.XType == XType.None || value.Value is null)
        {
            return 0L; // Return a default value for None or null
        }

        if (value.XType == XType.Int || value.XType == XType.Long)
        {
            if (long.TryParse(value.Value.ToString(), out var number))
            {
                return number; // Successfully parsed as long
            }
        }
        else if (value.XType == XType.Bool)
        {
            return (bool)value.Value ? 1L : 0L; // Convert bool to 1 for true, 0 for false
        }
        else if (value.XType == XType.Decimal)
        {
            if (decimal.TryParse(value.Value.ToString(), out var decimalValue))
            {
                return (long)decimalValue; // Convert decimal to long
            }
        }
        else if (value.XType == XType.Double)
        {
            if (double.TryParse(value.Value.ToString(), out var doubleValue))
            {
                return (long)doubleValue; // Convert double to long
            }
        }

        return 0L; // Default value for unsupported types or failed parsing
    }

    public static double To_Double(this XValue value)
    {
        if (value.XType == XType.None || value.Value is null)
        {
            return 0.0; // Return a default value for None or null
        }

        if (value.XType == XType.Int || value.XType == XType.Long)
        {
            if (long.TryParse(value.Value.ToString(), out var number))
            {
                return (double)number; // Convert long to double
            }
        }
        else if (value.XType == XType.Bool)
        {
            return (bool)value.Value ? 1.0 : 0.0; // Convert bool to 1.0 for true, 0.0 for false
        }
        else if (value.XType == XType.Decimal)
        {
            if (decimal.TryParse(value.Value.ToString(), out var decimalValue))
            {
                return (double)decimalValue; // Convert decimal to double
            }
        }
        else if (value.XType == XType.Double)
        {
            if (double.TryParse(value.Value.ToString(), out var doubleValue))
            {
                return doubleValue; // Successfully parsed as double
            }
        }

        return 0.0; // Default value for unsupported types or failed parsing
    }

    public static decimal To_Decimal(this XValue value)
    {
        if (value.XType == XType.None || value.Value is null)
        {
            return 0m; // Return a default value for None or null
        }

        if (value.XType == XType.Int || value.XType == XType.Long)
        {
            if (long.TryParse(value.Value.ToString(), out var number))
            {
                return (decimal)number; // Convert long to decimal
            }
        }
        else if (value.XType == XType.Bool)
        {
            return (bool)value.Value ? 1m : 0m; // Convert bool to 1m for true, 0m for false
        }
        else if (value.XType == XType.Decimal)
        {
            if (decimal.TryParse(value.Value.ToString(), out var decimalValue))
            {
                return decimalValue; // Successfully parsed as decimal
            }
        }
        else if (value.XType == XType.Double)
        {
            if (double.TryParse(value.Value.ToString(), out var doubleValue))
            {
                return (decimal)doubleValue; // Convert double to decimal
            }
        }

        return 0m; // Default value for unsupported types or failed parsing
    }

    public static DateTime To_DateTime(this XValue value)
    {
        if (value.XType == XType.None || value.Value is null)
        {
            return default;
        }

        if (value.XType == XType.Date || value.XType == XType.String)
        {
            if (DateTime.TryParseExact(value.Value.ToString(), XConstant.DateTimeFormat, null,
                    System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                return parsedDateTime;
            }
        }

        return default;
    }

    public static List<long> To_LongList(this XValue value)
    {
        if (value.XType == XType.LongList)
        {
            if (value.Value is List<long> longList)
            {
                return longList;
            }
        }

        return new List<long>();
    }

    public static List<decimal> To_DecimalList(this XValue value)
    {
        if (value.XType == XType.DecimalList)
        {
            if (value.Value is List<decimal> decimalList)
            {
                return decimalList;
            }
        }

        return new List<decimal>();
    }

    public static List<double> To_DoubleList(this XValue value)
    {
        if (value.XType == XType.DoubleList)
        {
            if (value.Value is List<double> doubleList)
            {
                return doubleList;
            }
        }

        return new List<double>();
    }

    public static List<bool> To_BoolList(this XValue value)
    {
        if (value.XType == XType.BoolList)
        {
            if (value.Value is List<bool> boolList)
            {
                return boolList;
            }
        }

        return new List<bool>();
    }

    public static List<string> To_StringList(this XValue value)
    {
        if (value.XType == XType.StringList)
        {
            if (value.Value is List<string> stringList)
            {
                return stringList;
            }
        }

        return new List<string>();
    }
    
    public static XBag To_Bag(this XValue value)
    {
        if (value.XType == XType.Bag)
        {
            if (value.Value is XBag bag)
            {
                return bag;
            }
        }

        return new XBag();
    }
    
    public static XTable To_Table(this XValue value)
    {
        if (value.XType == XType.Table)
        {
            if (value.Value is XTable table)
            {
                return table;
            }
        }

        return new XTable();
    }
}