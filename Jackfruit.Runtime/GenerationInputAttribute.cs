namespace Jackfruit.Runtime
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false) ]
    sealed class GenerationInputAttribute : Attribute
    {
        public GenerationInputAttribute()
        {
        }

    }
}
