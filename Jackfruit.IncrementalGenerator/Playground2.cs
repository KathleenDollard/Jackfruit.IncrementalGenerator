namespace Jackfruit.IncrementalGenerator
{
    // **** User written code
    public class Program
    {
        public static void Main(string[] args)
        {
            ConsoleApp.AddRootCommand(A);
            ConsoleApp.A.AddSubCommand(B);

            // if you do not want to customize anything
            ConsoleApp.Run(args);

            // for customization
            var app = ConsoleApp.Create();
            app.RootCommand.B.Description = "Thing1";
            app.RootCommand.B.Description = "Thing2";
        }

        public static int A() => 3;
        public static int B() => 5;
    }

    // **** Not generated library code
    public class CliCommandBase
    {
        public string Description { get; set; }
    }
    public class ConsoleAppBase
    {
        public static int Run(string[] args) => 42;
    }
    // If we use a base class for classes A and  B, the IDE offers to simplify the call to a direct call
    //public class CliDef
    //{
    //    public static void AddSubCommand(Delegate x) { }
    //}


    // **** Generated (with a lot more detail)
    public class ConsoleApp : ConsoleAppBase
    {
        public static void AddRootCommand(Delegate x) { }
        public static ConsoleApp Create()
            => new ConsoleApp();
        private ConsoleApp() { RootCommand = new ACommand(); }
        public ACommand RootCommand { get; }


        public static class A
        {
            public static void AddSubCommand(Delegate x) { }
            public class B
            {
                public static void AddSubCommand(Delegate x) { }
            }
        }
    }

    public partial class ACommand : CliCommandBase
    {
        public ACommand() { B = new BCommand(); }
        public BCommand B { get; }
        // all the normal System.CommandLine support generation
    }
    public partial class BCommand : CliCommandBase
    {
        public BCommand() { }
        // all the normal System.CommandLine support generation
    }

}
