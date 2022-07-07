using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Completions;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Runtime.InteropServices.ComTypes;

#nullable enable
namespace Jackfruit.Internal
{

    // *** Assume everything below in this file will be in a library after we settle ****
    public abstract class GeneratedCommandBase<TSelf, TResult, TParent> : GeneratedCommandBase<TSelf, TResult>
        where TSelf : GeneratedCommandBase<TSelf, TResult>
        where TParent : GeneratedCommandBase
    {
        protected TParent? Parent { get; set; }

        public override void Validate(InvocationContext invocationContext)
        {
            base.Validate(invocationContext);
            if (Parent is not null)
            { Parent.Validate(invocationContext); }
        }
    }

    public abstract class GeneratedCommandBase<TSelf, TResult> : GeneratedCommandBase
        where TSelf : GeneratedCommandBase
    {
        public override void Validate(InvocationContext invocationContext)
        {
            base.Validate(invocationContext);
        }
    }

    public interface IRootCommand { }

    public abstract class RootCommand<T, TResult> : GeneratedCommandBase<T, TResult>, IRootCommand
        where T : RootCommand<T, TResult>, new()
    {
        public static RootCommand<T, TResult> Create(Delegate runHandler, params SubCommand[] cliRoot)
        { return new T(); }

        public int Run(string[] args) 
            => SystemCommandLineCommand.Invoke(args);
    }

    public abstract class GeneratedCommandBase
    {
        public string? Name { get; protected set; }
        private Command? _sytemCommandLineCommand = null;
        public Command SystemCommandLineCommand
        {
            get
            {
                if (_sytemCommandLineCommand is null)
                {
                    _sytemCommandLineCommand =
                        this is IRootCommand
                            ? new RootCommand()
                            : new Command(Name!);
                }
                return _sytemCommandLineCommand;
            }
        }

        // no op: for generation
        public void AddCommand(Delegate method) { }
        //public void AddCommands(params Delegate[] method) { }
        public void AddCommand<TAttachTo>(Delegate method) { }
        //public void AddCommands<TAttachTo>(params Delegate[] method) { }
        public virtual void Validate(InvocationContext invocationContext) { }
        public void AddValidator(Delegate action, params object[] values) { }

        protected void AddMessageOnFail(List<string> messages, string? newMessage)
        {
            if (string.IsNullOrWhiteSpace(newMessage))
            { return; }
            messages.Add(newMessage!);
        }

        protected void AddMessagesOnFail(List<string> messages, IEnumerable<string>? newMessages)
        {
            if (newMessages is not null && newMessages.Any())
            { messages.AddRange(newMessages); }
        }

        /// <summary>
        /// Adds a subcommand to the command.
        /// </summary>
        /// <param name="command">The subcommand to add to the command.</param>
        /// <remarks>Commands can be nested to an arbitrary depth.</remarks>
        protected void AddCommandToScl(GeneratedCommandBase command)
            => SystemCommandLineCommand.AddCommand(command.SystemCommandLineCommand);

        //*** Put SCL wrappers below. Avoiding regions to avoid antagonizing region haters
        /// <summary>
        /// Gets the set of strings that can be used on the command line to specify the symbol.
        /// </summary>
        public IReadOnlyCollection<string> Aliases => SystemCommandLineCommand.Aliases;

        public string? Description => SystemCommandLineCommand.Description;

        /// <summary>
        /// Determines whether the specified alias has already been defined.
        /// </summary>
        /// <param name="alias">The alias to search for.</param>
        /// <returns><see langword="true" /> if the alias has already been defined; otherwise <see langword="false" />.</returns>
        public bool HasAlias(string alias) => SystemCommandLineCommand.HasAlias(alias);

        /// <summary>
        /// Represents all of the arguments for the command.
        /// </summary>
        public IReadOnlyList<Argument> Arguments => SystemCommandLineCommand.Arguments;

        /// <summary>
        /// Represents all of the options for the command, including global options.
        /// </summary>
        public IReadOnlyList<Option> Options => SystemCommandLineCommand.Options;

        /// <summary>
        /// Represents all of the global options for the command
        /// </summary>
        public IReadOnlyList<Option> GlobalOptions => SystemCommandLineCommand.Options;


        /// <summary>
        /// Adds an alias to the command. Multiple aliases can be added to a command, most often used to provide a
        /// shorthand alternative.
        /// </summary>
        /// <param name="alias">The alias to add to the command.</param>
        public void AddAlias(string alias) => SystemCommandLineCommand.AddAlias(alias);

        /// <summary>
        /// Gets or sets a value that indicates whether unmatched tokens should be treated as errors. For example,
        /// if set to <see langword="true"/> and an extra command or argument is provided, validation will fail.
        /// </summary>
        public bool TreatUnmatchedTokensAsErrors { get; set; } = true;

        /// <summary>
        /// Represents all of the symbols for the command.
        /// </summary>
        public IEnumerator<Symbol> GetEnumerator() => SystemCommandLineCommand.GetEnumerator();

        /// <summary>
        /// Gets completions for the symbol.
        /// </summary>
        public IEnumerable<CompletionItem> GetCompletions(CompletionContext context) => SystemCommandLineCommand.GetCompletions(context);

        /// <summary>
        /// Adds an <see cref="Option"/> to the command.
        /// </summary>
        /// <param name="option">The option to add to the command.</param>
        protected void Add(Option option) => SystemCommandLineCommand.AddOption(option);

        /// <summary>
        /// Adds an <see cref="Argument"/> to the command.
        /// </summary>
        /// <param name="argument">The argument to add to the command.</param>
        protected void Add(Argument argument) => SystemCommandLineCommand.AddArgument(argument);

        /// <summary>
        /// Gets or sets the <see cref="ICommandHandler"/> for the command. The handler represents the action
        /// that will be performed when the command is invoked.
        /// </summary>
        /// <remarks>
        /// <para>Use one of the <see cref="Handler.SetHandler(Command, Action)" /> overloads to construct a handler.</para>
        /// <para>If the handler is not specified, parser errors will be generated for command line input that
        /// invokes this command.</para></remarks>
        protected ICommandHandler? Handler
        {
            get => SystemCommandLineCommand.Handler;
            set => SystemCommandLineCommand.Handler = value;
        }

        //protected static T? GetValueForHandlerParameter<T>(
        //    IValueDescriptor<T> symbol,
        //    InvocationContext context)
        //{
        // @jon: Can you explain this code from SCL Handler partial? When will this be an IValueSource?
        //if (symbol is IValueSource valueSource &&
        //    valueSource.TryGetValue(symbol, context.BindingContext, out var boundValue) &&
        //    boundValue is T value)
        //{
        //    return value;
        //}
        //else
        //{
        //    return symbol switch
        //    {
        //        Argument<T> argument => context.ParseResult.CommandResult.GetValueForArgument(argument),
        //        Option<T> option => context.ParseResult.CommandResult.GetValueForOption(option),
        //        _ => throw new ArgumentOutOfRangeException()
        //    };
        //}
        //}

        protected static T? GetValueForSymbol<T>(IValueDescriptor<T> symbol, CommandResult result)
            => symbol switch
            {
                Argument<T> argument => result.GetValueForArgument(argument),
                Option<T> option => result.GetValueForOption(option),
                _ => throw new ArgumentOutOfRangeException()
            };


        protected static T? GetService<T>(InvocationContext invocationContext)
                   where T : class
        {
            var typeT = typeof(T);
            return typeT.IsAssignableFrom(typeof(IConsole))
                ? (T)invocationContext.Console
                : GetService(invocationContext);

            static T? GetService(InvocationContext invocationContext)
             {
                var service = invocationContext.BindingContext.GetService(typeof(T));
                return service is null
                    ? null
                    : (T)service;
            }
        }
    }

    public class EmptyCommand : GeneratedCommandBase
    { }
}

