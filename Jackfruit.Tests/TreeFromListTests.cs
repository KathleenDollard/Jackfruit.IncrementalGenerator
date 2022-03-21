using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using System.Collections.Generic;
using Jackfruit.Models;
using System.Linq;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class TreefromListTests
    {

        [Fact]
        public void Single_item_gives_no_subcommands()
        {
            var commandDefs = new List<CommandDef>
            {
                new CommandDef("A",  new string[] {"A"})
            };
            var actual = commandDefs.TreeFromList(1);

            Assert.Empty(actual.SubCommands);
            Assert.Equal("A", actual.Id);

        }

        [Fact]
        public void Single_nesting_gives_one_subcommand()
        {
            var commandDefs = new List<CommandDef>
            {
                new CommandDef("A",  new string[] {"A"}),
                new CommandDef("B",  new string[] {"A","B"})
            };
            var actual = commandDefs.TreeFromList(1);

            Assert.Equal("A", actual.Id);
            Assert.Single(actual.SubCommands);
            Assert.Equal("B", actual.SubCommands.Single().Id);

        }

        [Fact]
        public void Two_nesting_layers_gives_one_subcommand_each()
        {
            var commandDefs = new List<CommandDef>
            {
                new CommandDef("A",  new string[] {"A"}),
                new CommandDef("B",  new string[] {"A","B"}),
                new CommandDef("C",  new string[] {"A","B","C"})
            };
            var actual = commandDefs.TreeFromList(1);

            Assert.Equal("A", actual.Id);
            Assert.Single(actual.SubCommands);
            Assert.Equal("B", actual.SubCommands.Single().Id);
            Assert.Single(actual.SubCommands.Single().SubCommands);
            Assert.Equal("C", actual.SubCommands.Single().SubCommands.Single().Id);
        }

        [Fact]
        public void Two_commandDefs_in_layer_are_output()
        {
            var commandDefs = new List<CommandDef>
            {
                new CommandDef("A",  new string[] {"A"}),
                new CommandDef("B",  new string[] {"A","B"}),
                new CommandDef("C",  new string[] {"A","C"})
            };
            var actual = commandDefs.TreeFromList(1);

            Assert.Equal("A", actual.Id);
            Assert.Equal(2,actual.SubCommands.Count());
            Assert.Equal("B", actual.SubCommands.First().Id);
            Assert.Equal("C", actual.SubCommands.Skip(1).Single().Id);
        }


        [Fact]
        public void Complex_tree_output()
        {
            var commandDefs = new List<CommandDef>
            {
                new CommandDef("A",  new string[] {"A"}),
                new CommandDef("B",  new string[] {"A","B"}),
                new CommandDef("D",  new string[] {"A","B","D"}),
                new CommandDef("E",  new string[] {"A","B","E"}),
                new CommandDef("F",  new string[] {"A","B","F"}),
                new CommandDef("C",  new string[] {"A","C"}),
                new CommandDef("G",  new string[] {"A","C","G"}),
                new CommandDef("H",  new string[] {"A","C","H"}),
                new CommandDef("I",  new string[] {"A","I"}),
                new CommandDef("J",  new string[] {"A","I","J"}),
                new CommandDef("K",  new string[] {"A","I","J","K"})
            };
            var actual = commandDefs.TreeFromList(1);

            Assert.Equal(new List<string> { "B", "C", "I", }, actual.SubCommands.Select(x => x.Id).ToList());
            Assert.Equal(new List<string> { "D", "E", "F", }, actual.SubCommands.First().SubCommands.Select(x => x.Id).ToList());
            Assert.Equal(new List<string> { "G", "H",  }, actual.SubCommands.Skip(1).First().SubCommands.Select(x => x.Id).ToList());
            Assert.Equal(new List<string> { "J",  }, actual.SubCommands.Skip(2).First().SubCommands.Select(x => x.Id).ToList());
            Assert.Equal(new List<string> { "K", }, actual.SubCommands.Skip(2).First().SubCommands.First().SubCommands.Select(x => x.Id).ToList());



        }
    }
}
