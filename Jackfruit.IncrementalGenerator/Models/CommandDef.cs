namespace Jackfruit.Models
{
    public class CommandDef
    {
        public CommandDef(
            string id,
            IEnumerable<string> path)
        {
            Id = id;
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
            string uniqueId,
            string nspace,
            IEnumerable<string> path,
            string? description,
            string[] aliases,
            IEnumerable<MemberDef> members,
            string handlerMethodName,
            IEnumerable<CommandDef> subCommands,
            string returnType
            )
        {
            Id = id;
            UniqueId = uniqueId;
            Namespace = nspace;
            Description = description;
            Aliases = aliases;
            Members = members;
            HandlerMethodName = handlerMethodName;
            SubCommands = subCommands;
            Path = path;
            ReturnType = returnType;
        }

        public string Id { get; }
        public string UniqueId { get; }
        public string Namespace { get; }
        public string? Description { get; }
        public string[] Aliases { get; }
        //Options, args, and services in order of handler parameters
        public IEnumerable<MemberDef> Members { get; }
        public string HandlerMethodName { get; }
        public IEnumerable<CommandDef> SubCommands { get; set; }
        public IEnumerable<string> Path { get; }
        public string ReturnType { get; }

    }
}
