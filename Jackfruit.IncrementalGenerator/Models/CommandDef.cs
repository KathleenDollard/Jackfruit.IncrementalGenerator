using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Jackfruit.Models
{
    public abstract record CommandDefBase
    { }

    public record EmptyCommandDef : CommandDefBase
    { }


    public record CommandDef : CommandDefBase
    {
        public CommandDef(
            string id,
            IEnumerable<string> path)
        {
            Id = id;
            Name = id;
            UniqueId = string.Join("|", path);
            Namespace = "";
            Description = null;
            Aliases = new string[] { };
            Members = new List<MemberDef>();
            HandlerMethodName = "";
            SubCommands = new List<CommandDef>(); ;
            Path = path;
            ReturnType = "";
        }

        public CommandDef(
            string id,
            string name,
            string uniqueId,
            string nspace,
            IEnumerable<string> path,
            CommandDef? parent,
            string? description,
            string[] aliases,
            IEnumerable<MemberDef> members,
            string handlerMethodName,
            IEnumerable<CommandDef> subCommands,
            string returnType
            )
        {
            Id = id;
            Name = name;
            UniqueId = uniqueId;
            Namespace = nspace;
            Parent = parent;
            Description = description;
            Aliases = aliases;
            Members = members;
            HandlerMethodName = handlerMethodName;
            SubCommands = subCommands;
            Path = path;
            ReturnType = returnType;
        }

        public string Id { get; }
        public string Name { get; internal set; }
        public string UniqueId { get; }
        public string Namespace { get; }
        public CommandDef? Parent { get; }
        public ValidatorDef? Validator { get; internal set; }
        public string? Description { get; }
        public string[] Aliases { get; }
        //Options, args, and services in order of handler parameters
        public IEnumerable<MemberDef> Members { get; }
        public string HandlerMethodName { get; }
        public IEnumerable<CommandDefBase> SubCommands { get; set; }
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
                SubCommands.SequenceEqual(other.SubCommands) &&
                GenerationStyleTags.SequenceEqual(other.GenerationStyleTags) &&
                Path.SequenceEqual(other.Path);

        public override int GetHashCode()
            => (Id,
                Name,
                UniqueId,
                Namespace,
                Parent,
                Validator,
                Description,
                HandlerMethodName,
                ReturnType,
                string.Join(",", Aliases),
                string.Join(",", Members),
                string.Join(",", SubCommands),
                string.Join(",", GenerationStyleTags),
                string.Join(",", Path)).GetHashCode();

    }
}
