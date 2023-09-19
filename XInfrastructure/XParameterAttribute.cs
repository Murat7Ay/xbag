namespace XInfrastructure;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class XParameterAttribute : Attribute
{
    public string Key { get; }
    public XType XType { get; }
    public XParameterType XParameter { get; }
    public string Description { get; }

    public XParameterAttribute(string key, XType xType, XParameterType xParameter, string description)
    {
        XType = xType;
        Key = key;
        XParameter = xParameter;
        Description = description;
    }

}