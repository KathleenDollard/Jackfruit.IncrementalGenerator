
namespace Jackfruit.Common
{
    public abstract record MemberDef
    {
        protected MemberDef(string id, string name, string description, string? typeName)
        {
            Id = id;
            Name = name;
            Description = description;
            TypeName = typeName ?? "System.Boolean";
        }

        public string Id { get; }
        public string Description { get; }
        public string TypeName { get; }

        public string Name { get; set; }

        // @samharwell This does not seem the right way to handle these
        public virtual bool Equals(MemberDef other)
            =>
                Id == other.Id &&
                Description == other.Description &&
                TypeName == other.TypeName &&
                Name == other.Name;

        public override int GetHashCode()
            => (Id, Description, TypeName, Name).GetHashCode();
    }
    public record OptionDef : MemberDef
    {
        public OptionDef(
            string id,
            string name,
            string description,
            string? typeName,
            IEnumerable<string> aliases,
            string argDisplayName,
            bool required)
            : base(id, name, description, typeName)
        {
            ArgDisplayName = argDisplayName;
            Aliases = aliases;
            Required = required;
        }

        public string ArgDisplayName { get; }
        public IEnumerable<string> Aliases { get; }
        public bool Required { get; }

        public virtual bool Equals(OptionDef other)
            =>  base.Equals(other) &&
                ArgDisplayName == other.ArgDisplayName &&
                Required == other.Required &&
                Aliases.SequenceEqual(other.Aliases);

        public override int GetHashCode()
        {
            var hash = 13;
            unchecked
            {
                hash += base.GetHashCode();
                hash += (ArgDisplayName, Required).GetHashCode();
                foreach (var sub in Aliases)
                { hash += sub.GetHashCode(); }
            }
            return hash;
        }
    }
    public record ArgumentDef : MemberDef
    {
        public ArgumentDef(
            string id,
            string name,
            string description,
            string? typeName,
            bool required)
            : base(id, name, description, typeName)
        {
            Required = required;
        }

        public bool Required { get; }

        public virtual bool Equals(ArgumentDef other)
            => base.Equals(other) &&
                Required == other.Required;
        public override int GetHashCode()
        {
            var hash = 17;
            unchecked
            {
                hash += base.GetHashCode();
                hash += Required.GetHashCode();
            }
            return hash;
        }
    }
    public record ServiceDef : MemberDef
    {
        public ServiceDef(
            string id,
            string name,
            string description,
            string? typeName)
            : base(id, name, description, typeName)
        { }
    }

    public record UnknownMemberDef : MemberDef
    {
        public UnknownMemberDef(
            string id)
            : base(id, id, "", "")
        { }
    }
}
