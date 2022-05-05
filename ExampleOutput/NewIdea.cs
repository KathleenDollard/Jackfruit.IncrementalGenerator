using DemoHandlersUpdated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleOutput
{
    public class CliNode
    {
        public Delegate Action;
        public IEnumerable<CliNode> SubCommands;

        public CliNode(Delegate action, List<CliNode> subCommands)
        {
            Action = action;
            SubCommands = subCommands;
        }
        public CliNode(Delegate action)
        {
            Action = action;
            SubCommands = new List<CliNode>();
        }
    }

    internal class Program2
    {
        public CliNode Create2(CliNode rootNode)
        { throw new NotImplementedException(); }

        public int Main2(string[] args)
        {
            var root = Create2(
                 new(
                     Handlers.Franchise, new()
                     {  new(
                         Handlers.StarTrek,                          new()
                            { new( Handlers.NextGeneration,new()
                                  { new( Handlers.DeepSpaceNine),
                                    new(Handlers.Voyager)})
                                  })

                            }));
            return 0;
        }
    }

}
