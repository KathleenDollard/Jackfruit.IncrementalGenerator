using Microsoft.CodeAnalysis;

namespace Jackfruit.Common
{
    public static class Extensions
    {
        public static IncrementalValuesProvider<TItem> WhereNotNull<TItem>(this IncrementalValuesProvider<TItem> enumerable)
              =>  enumerable.Where(item => item is not null);
    }
}
