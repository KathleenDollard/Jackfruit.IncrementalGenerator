using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.Models;

namespace Jackfruit.IncrementalGenerator
{
    internal class Playground
    {
        // When we find no CreatefromRoot, generate non-specific ConsoleApplication
        public const string ConsoleClass = @"
using System;

namespace Jackfruit
{
    public class ConsoleApplication
    {
        private ConsoleApplication() {}
        public static ConsoleApplication AddRootCommand(Delegate rootCommandHandler) { }
    }
}
";

        // And when we do find a CreatefromRoot, generate specific ConsoleApplication (the root command here is lunch)
        public const string ConsoleClass2 = @"
using System;

namespace Prototype
{
    public class ConsoleApplication
    {
        private ConsoleApplication() {}
        public static ConsoleApplication AddRootCommand(Delegate rootCommandHandler) { } // This is this ConsoleApplication, not the general one
        public static LunchCommand CliRoot{get;}
    }

}
";


        // Nest classes to encourage use of the entire path, which we need as we only have syntax here.
        //    Namespaces could achieve the same, but would encourage usings
        //    Instance methods might seem logical but we would not have enough context to generate
        public const string GeneratedCli = @"
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using CommandBase;
using CliApp;
namespace Prototype
{
   
   public partial class LunchCommand : CliRootCommand, ICommandHandler
   {
      private LunchCommand()
      {
      }
      public static LunchCommand Create()
      {
         var command = new LunchCommand();
         command.RestaurantArgument = new Argument<string>(""restaurant"");
         command.Add(command.RestaurantArgument);
         command.Burger = BurgerCommand.Create();
         command.Add(command.Burger);
         command.Handler = command;
         return command;
      }
    public Argument<string> RestaurantArgument { get; set; }
    public string RestaurantArgumentResult(InvocationContext context)
    {
        return context.ParseResult.GetValueForOption<string>(RestaurantArgument);
    }
    public BurgerCommand Burger { get; set; }
    public Task<int> InvokeAsync(InvocationContext context)
    {
        Prototype.Handlers.Lunch(RestaurantArgumentResult(context));
        return Task.FromResult(context.ExitCode);
    }

    public partial class BurgerCommand : CliCommand, ICommandHandler
    {
        private BurgerCommand()
        : base(""Burger"")
        {
        }
        public static BurgerCommand Create()
        {
            var command = new BurgerCommand();
            command.Handler = command;
            return command;
        }
        public Task<int> InvokeAsync(InvocationContext context)
        {
            Prototype.Handlers.Burger();
            return Task.FromResult(context.ExitCode);
        }
    }
}
";

        public const string Usage = @"
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using CommandBase;
using CliApp;
namespace Prototype
{
    public static int Main(string[] args)
    {
        var app = ConsoleApplication.AddRootCommand(Lunch);
        var command = ConsoleApplication.Lunch.AddSubcommand(Burger); //you only need this value if you are further customizing
        return app.run(context);
    }
}
";


        //public CodeFileModel FirstFile(CommandDef commandDef)
        //=> new(commandDef.UniqueId)
        //{
        //    Usings = UsingModel.Create("System", "System.CommandLine"),
        //    Namespace = new(commandDef.Namespace)
        //    {
        //        Classes = ClassModel.Create(
        //            AppClass(commandDef),
        //            RootCommand(commandDef))
        //    }
        //};

        //private ClassModel RootCommand(CommandDef commandDef)
        //{
        //    throw new NotImplementedException();
        //}

        //private ClassModel AppClass(CommandDef commandDef)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
