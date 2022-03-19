namespace Jackfruit.Models
{
    public abstract class MemberDef
    {
        protected MemberDef(string id, string description, string typeName)
        {
            Id = id;
            Description = description;
            TypeName = typeName;
        }

        public string Id { get; }
        public string Description { get; }
        public string TypeName { get; }

        public string Name => Id;

    }
    public class OptionDef : MemberDef
    {
        public OptionDef(
            string id,
            string description,
            string typeName,
            IEnumerable<string> aliases,
            string argDisplayName,
            bool required)
            : base(id, description, typeName)
        {
            ArgDisplayName = argDisplayName;
            Aliases = aliases;
            Required = required;
        }

        public string ArgDisplayName { get; }
        public IEnumerable<string> Aliases { get; }
        public bool Required { get; }

    }
    public class ArgumentDef : MemberDef
    {
        public ArgumentDef(
            string id,
            string description,
            string typeName,
            bool required)
            : base(id, description, typeName)
        {
            Required = required;
        }

        public bool Required { get; }

    }
    public class ServiceDef : MemberDef
    {
        public ServiceDef(
            string id,
            string description,
            string typeName)
            : base(id, description, typeName)
        { }
    }
}