namespace Jackfruit.Common
{
    public abstract record CommandDefBase
    {
    }

    public record EmptyCommandDef : CommandDefBase
    {
    }


    public record CommandDef : CommandDefBase
    {
        //public CommandDef(
        //    string id,
        //    IEnumerable<string> path)
        //{
        //    Id = id;
        //    Name = id;
        //    UniqueId = string.Join("|", path);
        //    Namespace = "";
        //    Description = null;
        //    Aliases = new string[] { };
        //    Members = new List<MemberDef>();
        //    HandlerMethodName = "";
        //    SubCommandNames = new List<string>(); ;
        //    Path = path;
        //    ReturnType = "";
        //}

        public CommandDef(
            string id,
            string name,
            string uniqueId,
            string nspace,
            IEnumerable<string> path,
            string? parent,
            bool isParentRoot,
            string? description,
            string[] aliases,
            IEnumerable<MemberDef> members,
            string handlerMethodName,
            IEnumerable<string> subCommandNames,
            string returnType
            )
        {
            Id = id;
            Name = name;
            UniqueId = uniqueId;
            Namespace = nspace;
            Parent = parent;
            IsParentRoot = isParentRoot;
            Description = description;
            Aliases = aliases;
            Members = members;
            MyMembers= members.Where(m=>!m.IsOnRoot).ToList();
            HandlerMethodName = handlerMethodName;
            SubCommandNames = subCommandNames;
            Path = path;
            ReturnType = returnType;
        }

        public string Id { get; }
        public string Name { get; set; }
        public string UniqueId { get; }
        public string Namespace { get; }
        public string? Parent { get; }
        public bool IsParentRoot { get; }
        public ValidatorDef? Validator { get; set; }
        public string? Description { get; }
        public string[] Aliases { get; }
        //Options, args, and services in order of handler parameters
        public IEnumerable<MemberDef> Members { get; }
        public IEnumerable<MemberDef> MyMembers { get; }
        public string HandlerMethodName { get; }
        public IEnumerable<string> SubCommandNames { get; set; }
        public IEnumerable<string> Path { get; }
        public string ReturnType { get; }
        public Dictionary<string, object> GenerationStyleTags { get; } = new Dictionary<string, object>();

        public virtual bool Equals(CommandDef other)
            => base.Equals(other) &&
                Id == other.Id &&
                Name == other.Name &&
                UniqueId == other.UniqueId &&
                Namespace == other.Namespace &&
                Parent == other.Parent &&
                Validator == other.Validator &&
                Description == other.Description &&
                HandlerMethodName == other.HandlerMethodName &&
                ReturnType == other.ReturnType &&
                Aliases.SequenceEqual(other.Aliases) &&
                Members.SequenceEqual(other.Members) &&
                SubCommandNames.SequenceEqual(other.SubCommandNames) &&
                GenerationStyleTags.SequenceEqual(other.GenerationStyleTags) &&
                Path.SequenceEqual(other.Path);

        public override int GetHashCode()
        {
            var hash = 17;
            checked
            {
                hash += base.GetHashCode();
                hash += (Id,
                         Name,
                         UniqueId,
                         Namespace,
                         Parent,
                         Validator,
                         Description,
                         HandlerMethodName,
                         ReturnType).GetHashCode();
                foreach (var x in Aliases)
                { hash += x.GetHashCode(); }
                foreach (var x in Members)
                { hash += x.GetHashCode(); }
                foreach (var x in SubCommandNames)
                { hash += x.GetHashCode(); }
                foreach (var x in GenerationStyleTags)
                { hash += x.GetHashCode(); }
                foreach (var x in Path)
                { hash += x.GetHashCode(); }

            }
            return hash;
        }
    }
}
