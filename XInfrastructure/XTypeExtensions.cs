namespace XInfrastructure;

static class XTypeExtensions
{
    public static Type ToType(this XType xType)
    {
        return xType switch
        {
            XType.None => typeof(object),
            XType.Long => typeof(long),
            XType.Int => typeof(int),
            XType.String => typeof(string),
            XType.Bool => typeof(bool),
            XType.Decimal => typeof(decimal),
            XType.Double => typeof(double),
            XType.LongList => typeof(List<long>),
            XType.StringList => typeof(List<string>),
            XType.BoolList => typeof(List<bool>),
            XType.DecimalList => typeof(List<decimal>),
            XType.DoubleList => typeof(List<double>),
            XType.Date => typeof(DateTime),
            XType.Table => typeof(XTable),
            XType.Bag => typeof(XBag),
            _ => throw new NotSupportedException("Unsupported XType"),
        };
    }
}