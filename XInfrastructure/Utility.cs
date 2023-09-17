namespace XInfrastructure;

public static class Utility
{
    public static bool IsValidJsonPropertyName(string s)
    {
        return !string.IsNullOrEmpty(s) && char.IsLetter(s[0]) &&
               s.All(c => char.IsLetterOrDigit(c) || c is '_' or '$');
    }
}