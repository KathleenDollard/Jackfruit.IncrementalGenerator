using Jackfruit;
using Temp;

var x = RootCommand.Create(SubCommand.Create(Class1.Hello));
x.Run(args);
