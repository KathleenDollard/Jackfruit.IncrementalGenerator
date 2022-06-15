using Microsoft.CodeAnalysis;

namespace Jackfruit.IncrementalGenerator
{
    internal static class Extensions
    {
        public static IncrementalValuesProvider<TItem> WhereNotNull<TItem>(this IncrementalValuesProvider<TItem> enumerable)
              =>  enumerable.Where(item => item is not null);
    }
}
