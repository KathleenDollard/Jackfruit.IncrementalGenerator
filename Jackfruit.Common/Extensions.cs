using Microsoft.CodeAnalysis;
using System.Text;

namespace Jackfruit.Common
{
    public static class Extensions
    {
        public static IncrementalValuesProvider<TItem> WhereNotNull<TItem>(this IncrementalValuesProvider<TItem> enumerable)
              =>  enumerable.Where(item => item is not null);

        public static string ToKebabCase(this string value)
            // This is copied from System.CommandLine.DragonFruit. We should consider a canonical location.
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var sb = new StringBuilder();
            int i = 0;
            bool addDash = false;

            // handles beginning of string, breaks on first letter or digit. addDash might be better named "canAddDash"
            for (; i < value.Length; i++)
            {
                char ch = value[i];
                if (char.IsLetterOrDigit(ch))
                {
                    addDash = !char.IsUpper(ch);
                    sb.Append(char.ToLowerInvariant(ch));
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
                    if (addDash)
                    {
                        addDash = false;
                        sb.Append('-');
                    }

                    sb.Append(char.ToLowerInvariant(ch));
                }
                else if (char.IsLetterOrDigit(ch))
                {
                    addDash = true;
                    sb.Append(ch);
                }
                else //this coverts all non letter/digits to dash - specifically periods and underscores. Is this needed?
                {
                    addDash = false;
                    sb.Append('-');
                }
            }

            return sb.ToString();
        }

    }
}
