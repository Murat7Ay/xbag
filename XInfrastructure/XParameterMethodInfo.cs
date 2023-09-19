namespace XInfrastructure;

public class XParameterMethodInfo
{
    public string? Assembly { get; set; } 
    public string? TypeName { get; set; } 
    public string? MethodName { get; set; }
    public XMethodAttribute MethodInfo { get; set; }
    public List<XParameterAttribute> Inputs { get; set; } = new();
    public List<XParameterAttribute> Outputs { get; set; } = new();
}