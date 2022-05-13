using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jackfruit.Models
{
    public abstract class CommandDefBase
    { }

    public  class EmptyCommandDef:CommandDefBase
    { }


    public class CommandDef : CommandDefBase
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
        public string? Description { get; }
        public string[] Aliases { get; }
        //Options, args, and services in order of handler parameters
        public IEnumerable<MemberDef> Members { get; }
        public string HandlerMethodName { get; }
        public IEnumerable<CommandDefBase> SubCommands { get; set; }
        public IEnumerable<string> Path { get; }
        public string ReturnType { get; }
        public Dictionary<string, object> GenerationStyleTags { get; } = new Dictionary<string, object>();

    }
}
