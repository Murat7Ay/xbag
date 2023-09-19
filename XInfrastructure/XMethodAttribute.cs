namespace XInfrastructure;

[AttributeUsage(AttributeTargets.Method)]
public class XMethodAttribute : Attribute
{
    public string Name { get; }
    public XMethodType XMethod { get; }
    public string Description { get; }

    public XMethodAttribute(string name, XMethodType xMethodType, string description)
    {
        Name = name;
        XMethod = xMethodType;
        Description = description;
    }

}