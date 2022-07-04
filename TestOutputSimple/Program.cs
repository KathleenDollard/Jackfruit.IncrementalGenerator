using Jackfruit;
using Temp;

var x = RootCommand.Create(CommandNode.Create(Class1.Hello));
x.Run(args);
