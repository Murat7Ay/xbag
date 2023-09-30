public static class Extension
{
    public static bool IsInheritedFrom(this Type type, Type abs)
    {
        var baseType = type.BaseType;
        if (baseType == null)
            return false;

        if (baseType.IsGenericType
            && baseType.GetGenericTypeDefinition() == abs)
            return true;

        return baseType.IsInheritedFrom(abs);
    }
}