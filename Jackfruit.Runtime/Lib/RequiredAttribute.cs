namespace Jackfruit
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class RequiredAttribute : Attribute
    {
        public RequiredAttribute(bool isRequired = false)
        {
            IsRequired = isRequired;
        }

        public bool IsRequired { get; }
    }
}
