namespace Jackfruit
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class OptionArgumentNameAttribute : Attribute
    {
        public OptionArgumentNameAttribute(string argumentName)
        {
            ArgumentName = argumentName;
        }

        public string ArgumentName { get; }
    }
}
