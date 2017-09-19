using System;
using JetBrains.Annotations;

namespace Arbor.Specifications.NUnit
{
    internal static class StringExtensions
    {
        public static string EnsureStartsWithPrefix([NotNull] this string text, string prefix)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));
            }

            if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }

            return $"{prefix}{text}";
        }

        public static string EnsureStartsWithPrefix([NotNull] this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));
            }

            if (text.StartsWith("then it ", StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }

            if (text.StartsWith("it ", StringComparison.OrdinalIgnoreCase))
            {
                return $"then {text}";
            }

            return $"then it {text}";
        }

        public static string NormalizeTestName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return name.Replace("_", " ");
        }
    }
}
