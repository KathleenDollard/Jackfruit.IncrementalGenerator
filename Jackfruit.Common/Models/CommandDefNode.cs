using Jackfruit.IncrementalGenerator.CodeModels;
using System.Xml.Linq;
using System.Xml;
using System.Collections.ObjectModel;

namespace Jackfruit.Common
{
    public record CommandDefNode
    {
        public CommandDefNode(CommandDef commandDef)
        {
            CommandDef = commandDef;
        }
        public CommandDef CommandDef { get; }
        public List<CommandDefNode> SubCommandNodes { get; } = new();
        public void AddSubCommandNodes(IEnumerable<CommandDefNode> subCommandNode)
            => SubCommandNodes.AddRange(subCommandNode);

        public virtual bool Equals(CommandDefNode other)
          => base.Equals(other) &&
              CommandDef == other.CommandDef &&
              SubCommandNodes.SequenceEqual(other.SubCommandNodes);

        public override int GetHashCode()
        {
            var hash = 31;
            unchecked
            {
                hash += CommandDef.GetHashCode();
                foreach (var sub in SubCommandNodes)
                { hash += sub.GetHashCode(); }
            }
            return hash;
        }
    }
}
