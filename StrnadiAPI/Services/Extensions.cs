using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace StrnadiAPI.Services;

public static class Extensions
{
    public static IEnumerable<T> SelectWithout<T>(
        this IEnumerable<T> source,
        params Expression<Func<T, object>>[] excludedMembers)
        where T : new()
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (excludedMembers == null)
            throw new ArgumentNullException(nameof(excludedMembers));

        var excludedNames = new HashSet<string>(
            excludedMembers
                .Select(GetMemberName));

        foreach (var item in source)
        {
            var result = new T();
            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!excludedNames.Contains(property.Name) && property is { CanRead: true, CanWrite: true })
                {
                    var value = property.GetValue(item);
                    property.SetValue(result, value);
                }
            }

            yield return result;
        }
    }

    private static string GetMemberName<T>(Expression<Func<T, object>> expression)
    {
        if (expression.Body is MemberExpression member)
        {
            return member.Member.Name;
        }
        if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression memberOperand)
        {
            return memberOperand.Member.Name;
        }

        throw new ArgumentException("Invalid expression format. Must be a member access.", nameof(expression));
    }

    public static string Sha256(this string s)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        byte[] hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes); // C# 5.0+ (или BitConverter.ToString(hashBytes).Replace("-", "").ToLower())
    }
}