namespace Jackfruit
{
    [System.AttributeUsage(AttributeTargets.All, 
                    Inherited = false, AllowMultiple = true)]
    public sealed class AliasesAttribute : Attribute
    {
        public AliasesAttribute(params string[] aliases)
        {
            Values = aliases;
        }

        public string[] Values { get; }
    }
}
