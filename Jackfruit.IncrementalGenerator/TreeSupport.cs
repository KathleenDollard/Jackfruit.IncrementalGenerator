using Jackfruit.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator
{
    public static class TreeSupport
    {

        public static CommandDefBase TreeFromList(this IEnumerable<CommandDef> commandDefs, int pos = 0)
            => TreeFromListInternal(commandDefs, pos).FirstOrDefault() ?? new EmptyCommandDef();

        public static IEnumerable<CommandDefBase> TreeFromListInternal(this IEnumerable<CommandDef> commandDefs, int pos)
        {
            if (pos > 10) { throw new InvalidOperationException("Runaway recursion suspected"); }
            // This throws on badly formed trees. not sure whether to just let that happen and catch, or do more work here
            var roots = commandDefs.Where(x => GroupKey(x, pos) is null);

            foreach (var root in roots)
            {
                if (Helpers.GetStyle(root) != Helpers.Cli)
                {
                    var subCommands = ProcessRoot(pos, commandDefs, roots, root);
                    root.SubCommands = subCommands;
                }
            }


            return roots;

            static string? GroupKey(CommandDef commandDef, int pos)
                => commandDef.Path.Skip(pos).FirstOrDefault();

            static IEnumerable<CommandDefBase> ProcessRoot(int pos, IEnumerable<CommandDef> commandDefs, IEnumerable<CommandDef> roots, CommandDefBase root)
            {
                var subCommands = new List<CommandDefBase>();
                var remaining = commandDefs.Except(roots);
                if (remaining.Any())
                {
                    var groups = remaining.GroupBy(x => GroupKey(x, pos));
                    foreach (var group in groups)
                    {
                        var newSubCommands = group.TreeFromListInternal(pos + 1);
                        subCommands.AddRange(newSubCommands);
                    }
                }
                return subCommands;
            }
        }

    }
}
