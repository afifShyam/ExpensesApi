namespace ExpenseApi.Common;

public static class StringExtensions
{
    public static string ToCamelCasePath(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var parts = value.Split('.');

        for (var i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length > 0)
            {
                parts[i] = char.ToLowerInvariant(parts[i][0]) + parts[i][1..];
            }
        }

        return string.Join('.', parts);
    }
}