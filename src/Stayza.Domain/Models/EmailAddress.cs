using System.Text.RegularExpressions;

namespace Stayza.Domain.Models;

public record EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}
