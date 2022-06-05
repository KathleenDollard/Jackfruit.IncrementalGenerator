// This file is created by a generator.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.CommandLine.Binding;
using System.Threading.Tasks;
using Jackfruit.Internal;
using Jackfruit;

namespace DemoHandlers
{
   /// <summary>
   /// The wrapper class for the Franchise command.
   /// </summary>
   public partial class Franchise : GeneratedCommandBase<Franchise, Franchise.Result>, ICommandHandler
   {
      private Franchise() : base("Franchise")
      {
      }
      
      internal static Franchise Create()
      {
         var command = new Franchise();
         command.GreetingArgument = new Argument<string>("greetingArg");
         command.Add(command.GreetingArgument);
         command.StarTrek = StarTrek.Create(command);
         command.AddCommandToScl(command.StarTrek);
         command.SystemCommandLineCommand.AddValidator(command.Validate);
         command.Handler = command;
         return command;
      }
      
      /// <summary>
      /// The result class for the Franchise command.
      /// </summary>
      public class Result
      {
         internal Result(Franchise command, InvocationContext invocationContext) : this(command, invocationContext.ParseResult.CommandResult)
         {
         }
         
         internal Result(Franchise command, CommandResult commandResult)
         {
            Greeting = GetValueForSymbol(command.GreetingArgument, commandResult);
         }
         
         public string Greeting {get; set;}
      }
      
      /// <summary>
      /// Get an instance of the Result class for the Franchise command.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
      public override Result GetResult(InvocationContext invocationContext)
      {
         return new Result(this, invocationContext);
      }
      
      /// <summary>
      /// Get an instance of the Result class for the Franchise command that will not include any services.
      /// </summary>
      /// <param name="result">The System.CommandLine CommandResult used to retrieve values.</param>
      public override Result GetResult(CommandResult result)
      {
         return new Result(this, result);
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.Franchise(result.Greeting);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.Franchise(result.Greeting);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      /// <summary>
      /// The validate method invoked by System.CommandLine.
      /// </summary>
      /// <param name="commandResult">The System.CommandLine CommandResult used to retrieve values for validation and it will hold any errors.</param>
      public override void Validate(CommandResult commandResult)
      {
         base.Validate(commandResult);
         var result = GetResult(commandResult);
         var err = string.Join(Environment.NewLine, DemoHandlers.Validators.FranchiseValidate(result.Greeting));
         if (!(string.IsNullOrWhiteSpace(err)))
         {
            commandResult.ErrorMessage = err;
         }
      }
      
      public Argument<string> GreetingArgument {get; set;}
      public StarTrek StarTrek {get; set;}
   }
   
   /// <summary>
   /// The wrapper class for the StarTrek command.
   /// </summary>
   public partial class StarTrek : GeneratedCommandBase<StarTrek, StarTrek.Result, Franchise>, ICommandHandler
   {
      private StarTrek(Franchise parent) : base("StarTrek", parent)
      {
      }
      
      internal static StarTrek Create(Franchise parent)
      {
         var command = new StarTrek(parent);
         command.KirkOption = new Option<bool>("--Kirk");
         command.KirkOption.Description = "Whether to greet Captain Kirk";
         command.Add(command.KirkOption);
         command.SpockOption = new Option<bool>("--Spock");
         command.SpockOption.Description = "Whether to greet Spock";
         command.Add(command.SpockOption);
         command.UhuraOption = new Option<bool>("--Uhura");
         command.UhuraOption.Description = "Whether to greet Lieutenant Uhura";
         command.Add(command.UhuraOption);
         command.NextGeneration = NextGeneration.Create(command);
         command.AddCommandToScl(command.NextGeneration);
         command.SystemCommandLineCommand.AddValidator(command.Validate);
         command.Handler = command;
         return command;
      }
      
      /// <summary>
      /// The result class for the StarTrek command.
      /// </summary>
      public class Result
      {
         internal Result(StarTrek command, InvocationContext invocationContext) : this(command, invocationContext.ParseResult.CommandResult, command.Parent.GetResult(invocationContext))
         {
         }
         
         internal Result(StarTrek command, CommandResult result) : this(command, result, command.Parent.GetResult(result))
         {
         }
         
         private Result(StarTrek command, CommandResult commandResult, Franchise.Result parentResult)
         {
            Greeting = parentResult.Greeting;
            Kirk = GetValueForSymbol(command.KirkOption, commandResult);
            Spock = GetValueForSymbol(command.SpockOption, commandResult);
            Uhura = GetValueForSymbol(command.UhuraOption, commandResult);
         }
         
         public string Greeting {get; set;}
         public bool Kirk {get; set;}
         public bool Spock {get; set;}
         public bool Uhura {get; set;}
      }
      
      /// <summary>
      /// Get an instance of the Result class for the StarTrek command.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
      public override Result GetResult(InvocationContext invocationContext)
      {
         return new Result(this, invocationContext);
      }
      
      /// <summary>
      /// Get an instance of the Result class for the StarTrek command that will not include any services.
      /// </summary>
      /// <param name="result">The System.CommandLine CommandResult used to retrieve values.</param>
      public override Result GetResult(CommandResult result)
      {
         return new Result(this, result);
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.StarTrek(result.Greeting, result.Kirk, result.Spock, result.Uhura);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.StarTrek(result.Greeting, result.Kirk, result.Spock, result.Uhura);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      public Option<bool> KirkOption {get; set;}
      public Option<bool> SpockOption {get; set;}
      public Option<bool> UhuraOption {get; set;}
      public NextGeneration NextGeneration {get; set;}
   }
   
   /// <summary>
   /// The wrapper class for the NextGeneration command.
   /// </summary>
   public partial class NextGeneration : GeneratedCommandBase<NextGeneration, NextGeneration.Result, StarTrek>, ICommandHandler
   {
      private NextGeneration(StarTrek parent) : base("NextGeneration", parent)
      {
      }
      
      internal static NextGeneration Create(StarTrek parent)
      {
         var command = new NextGeneration(parent);
         command.PicardOption = new Option<bool>("--Picard");
         command.PicardOption.Description = "This is the description for Picard";
         command.PicardOption.AddAlias("-p");
         command.Add(command.PicardOption);
         command.DeepSpaceNine = DeepSpaceNine.Create(command);
         command.AddCommandToScl(command.DeepSpaceNine);
         command.Voyager = Voyager.Create(command);
         command.AddCommandToScl(command.Voyager);
         command.SystemCommandLineCommand.AddValidator(command.Validate);
         command.Handler = command;
         return command;
      }
      
      /// <summary>
      /// The result class for the NextGeneration command.
      /// </summary>
      public class Result
      {
         internal Result(NextGeneration command, InvocationContext invocationContext) : this(command, invocationContext.ParseResult.CommandResult, command.Parent.GetResult(invocationContext))
         {
         }
         
         internal Result(NextGeneration command, CommandResult result) : this(command, result, command.Parent.GetResult(result))
         {
         }
         
         private Result(NextGeneration command, CommandResult commandResult, StarTrek.Result parentResult)
         {
            Greeting = parentResult.Greeting;
            Kirk = parentResult.Kirk;
            Spock = parentResult.Spock;
            Uhura = parentResult.Uhura;
            Picard = GetValueForSymbol(command.PicardOption, commandResult);
         }
         
         public string Greeting {get; set;}
         public bool Kirk {get; set;}
         public bool Spock {get; set;}
         public bool Uhura {get; set;}
         public bool Picard {get; set;}
      }
      
      /// <summary>
      /// Get an instance of the Result class for the NextGeneration command.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
      public override Result GetResult(InvocationContext invocationContext)
      {
         return new Result(this, invocationContext);
      }
      
      /// <summary>
      /// Get an instance of the Result class for the NextGeneration command that will not include any services.
      /// </summary>
      /// <param name="result">The System.CommandLine CommandResult used to retrieve values.</param>
      public override Result GetResult(CommandResult result)
      {
         return new Result(this, result);
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.NextGeneration(result.Greeting, result.Picard);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.NextGeneration(result.Greeting, result.Picard);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      public Option<bool> PicardOption {get; set;}
      public DeepSpaceNine DeepSpaceNine {get; set;}
      public Voyager Voyager {get; set;}
   }
   
   /// <summary>
   /// The wrapper class for the DeepSpaceNine command.
   /// </summary>
   public partial class DeepSpaceNine : GeneratedCommandBase<DeepSpaceNine, DeepSpaceNine.Result, NextGeneration>, ICommandHandler
   {
      private DeepSpaceNine(NextGeneration parent) : base("DeepSpaceNine", parent)
      {
      }
      
      internal static DeepSpaceNine Create(NextGeneration parent)
      {
         var command = new DeepSpaceNine(parent);
         command.SiskoOption = new Option<bool>("--Sisko");
         command.Add(command.SiskoOption);
         command.OdoOption = new Option<bool>("--Odo");
         command.Add(command.OdoOption);
         command.DaxOption = new Option<bool>("--Dax");
         command.Add(command.DaxOption);
         command.WorfOption = new Option<bool>("--Worf");
         command.Add(command.WorfOption);
         command.OBrienOption = new Option<bool>("--OBrien");
         command.Add(command.OBrienOption);
         command.SystemCommandLineCommand.AddValidator(command.Validate);
         command.Handler = command;
         return command;
      }
      
      /// <summary>
      /// The result class for the DeepSpaceNine command.
      /// </summary>
      public class Result
      {
         internal Result(DeepSpaceNine command, InvocationContext invocationContext) : this(command, invocationContext.ParseResult.CommandResult, command.Parent.GetResult(invocationContext))
         {
         }
         
         internal Result(DeepSpaceNine command, CommandResult result) : this(command, result, command.Parent.GetResult(result))
         {
         }
         
         private Result(DeepSpaceNine command, CommandResult commandResult, NextGeneration.Result parentResult)
         {
            Greeting = parentResult.Greeting;
            Kirk = parentResult.Kirk;
            Spock = parentResult.Spock;
            Uhura = parentResult.Uhura;
            Picard = parentResult.Picard;
            Sisko = GetValueForSymbol(command.SiskoOption, commandResult);
            Odo = GetValueForSymbol(command.OdoOption, commandResult);
            Dax = GetValueForSymbol(command.DaxOption, commandResult);
            Worf = GetValueForSymbol(command.WorfOption, commandResult);
            OBrien = GetValueForSymbol(command.OBrienOption, commandResult);
         }
         
         public string Greeting {get; set;}
         public bool Kirk {get; set;}
         public bool Spock {get; set;}
         public bool Uhura {get; set;}
         public bool Picard {get; set;}
         public bool Sisko {get; set;}
         public bool Odo {get; set;}
         public bool Dax {get; set;}
         public bool Worf {get; set;}
         public bool OBrien {get; set;}
      }
      
      /// <summary>
      /// Get an instance of the Result class for the DeepSpaceNine command.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
      public override Result GetResult(InvocationContext invocationContext)
      {
         return new Result(this, invocationContext);
      }
      
      /// <summary>
      /// Get an instance of the Result class for the DeepSpaceNine command that will not include any services.
      /// </summary>
      /// <param name="result">The System.CommandLine CommandResult used to retrieve values.</param>
      public override Result GetResult(CommandResult result)
      {
         return new Result(this, result);
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      public Option<bool> SiskoOption {get; set;}
      public Option<bool> OdoOption {get; set;}
      public Option<bool> DaxOption {get; set;}
      public Option<bool> WorfOption {get; set;}
      public Option<bool> OBrienOption {get; set;}
   }
   
   /// <summary>
   /// The wrapper class for the Voyager command.
   /// </summary>
   public partial class Voyager : GeneratedCommandBase<Voyager, Voyager.Result, NextGeneration>, ICommandHandler
   {
      private Voyager(NextGeneration parent) : base("Voyager", parent)
      {
      }
      
      internal static Voyager Create(NextGeneration parent)
      {
         var command = new Voyager(parent);
         command.JanewayOption = new Option<bool>("--Janeway");
         command.Add(command.JanewayOption);
         command.ChakotayOption = new Option<bool>("--Chakotay");
         command.Add(command.ChakotayOption);
         command.TorresOption = new Option<bool>("--Torres");
         command.Add(command.TorresOption);
         command.TuvokOption = new Option<bool>("--Tuvok");
         command.Add(command.TuvokOption);
         command.SevenOfNineOption = new Option<bool>("--SevenOfNine");
         command.Add(command.SevenOfNineOption);
         command.SystemCommandLineCommand.AddValidator(command.Validate);
         command.Handler = command;
         return command;
      }
      
      /// <summary>
      /// The result class for the Voyager command.
      /// </summary>
      public class Result
      {
         internal Result(Voyager command, InvocationContext invocationContext) : this(command, invocationContext.ParseResult.CommandResult, command.Parent.GetResult(invocationContext))
         {
            Console = GetService<System.CommandLine.IConsole>(invocationContext);
         }
         
         internal Result(Voyager command, CommandResult result) : this(command, result, command.Parent.GetResult(result))
         {
         }
         
         private Result(Voyager command, CommandResult commandResult, NextGeneration.Result parentResult)
         {
            Greeting = parentResult.Greeting;
            Kirk = parentResult.Kirk;
            Spock = parentResult.Spock;
            Uhura = parentResult.Uhura;
            Picard = parentResult.Picard;
            Janeway = GetValueForSymbol(command.JanewayOption, commandResult);
            Chakotay = GetValueForSymbol(command.ChakotayOption, commandResult);
            Torres = GetValueForSymbol(command.TorresOption, commandResult);
            Tuvok = GetValueForSymbol(command.TuvokOption, commandResult);
            SevenOfNine = GetValueForSymbol(command.SevenOfNineOption, commandResult);
         }
         
         public string Greeting {get; set;}
         public bool Kirk {get; set;}
         public bool Spock {get; set;}
         public bool Uhura {get; set;}
         public bool Picard {get; set;}
         public System.CommandLine.IConsole Console {get; set;}
         public bool Janeway {get; set;}
         public bool Chakotay {get; set;}
         public bool Torres {get; set;}
         public bool Tuvok {get; set;}
         public bool SevenOfNine {get; set;}
      }
      
      /// <summary>
      /// Get an instance of the Result class for the Voyager command.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
      public override Result GetResult(InvocationContext invocationContext)
      {
         return new Result(this, invocationContext);
      }
      
      /// <summary>
      /// Get an instance of the Result class for the Voyager command that will not include any services.
      /// </summary>
      /// <param name="result">The System.CommandLine CommandResult used to retrieve values.</param>
      public override Result GetResult(CommandResult result)
      {
         return new Result(this, result);
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.Voyager(result.Console, result.Greeting, result.Janeway, result.Chakotay, result.Torres, result.Tuvok, result.SevenOfNine);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = GetResult(invocationContext);
         DemoHandlers.Handlers.Voyager(result.Console, result.Greeting, result.Janeway, result.Chakotay, result.Torres, result.Tuvok, result.SevenOfNine);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      public Option<bool> JanewayOption {get; set;}
      public Option<bool> ChakotayOption {get; set;}
      public Option<bool> TorresOption {get; set;}
      public Option<bool> TuvokOption {get; set;}
      public Option<bool> SevenOfNineOption {get; set;}
   }
   
}
