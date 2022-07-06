using Microsoft.CodeAnalysis;
using System.Text;

namespace Jackfruit.Common
{
    public static class Extensions
    {
        public static IncrementalValuesProvider<TItem> WhereNotNull<TItem>(this IncrementalValuesProvider<TItem> enumerable)
              => enumerable.Where(item => item is not null);

        public static string ToKebabCase(this string value)
            => ToSeparatedCase(value, '-', Casing.Lower);

        public static string ToScreamingSnakeCase(this string value)
            => ToSeparatedCase(value, '_', Casing.Upper);

        private enum Casing
        {
            Unchanged,
            Lower,
            Upper
        }

        private static string ToSeparatedCase(this string value, char separator, Casing casing)
        // This is copied from System.CommandLine.DragonFruit and varied for snake. We should consider a canonical location.
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var sb = new StringBuilder();
            int i = 0;
            bool canAddSep = false;

            // handles beginning of string, breaks on first letter or digit. 
            for (; i < value.Length; i++)
            {
                char ch = value[i];
                if (char.IsLetterOrDigit(ch))
                {
                    canAddSep = !char.IsUpper(ch);
                    sb.Append(NewChar(ch, casing));
                    i++;
                    break;
                }
            }

            // reusing i, start at the same place
            for (; i < value.Length; i++)
            {
                char ch = value[i];
                if (char.IsUpper(ch))
                {
                    if (canAddSep)
                    {
                        canAddSep = false;
                        sb.Append(separator);
                    }

                    sb.Append(NewChar(ch, casing));
                }
                else if (char.IsLetterOrDigit(ch))
                {
                    canAddSep = true;
                    sb.Append(NewChar(ch, casing));
                }
                else //this coverts all non letter/digits to dash - specifically periods and underscores. Is this needed?
                {
                    canAddSep = false;
                    sb.Append(separator);
                }
            }

            return sb.ToString();

            static char NewChar(char ch , Casing casing)
                => casing switch
                {
                    Casing.Lower => char.ToLowerInvariant(ch),
                    Casing.Upper => char.ToUpperInvariant(ch),
                    _ => ch
                };
        }

    }
}
