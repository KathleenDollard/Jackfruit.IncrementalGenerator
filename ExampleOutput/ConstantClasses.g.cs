using System.CommandLine;
using System.CommandLine.Completions;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

#nullable enable
namespace Jackfruit
{

    // *** Assume everything below in this file will be in a library after we settle ****
    public abstract class GeneratedCommandBase<TSelf, TResult, TParent> : GeneratedCommandBase<TSelf, TResult>
        where TSelf : GeneratedCommandBase<TSelf, TResult>
        where TResult : new()
        where TParent : GeneratedCommandBase
    {
        protected GeneratedCommandBase(string name, TParent parent, string? description = null)
            : base(name, description)
        {
            this.parent = parent;
        }

        private readonly TParent parent;
        protected TParent Parent => parent;

        public override void Validate(CommandResult commandResult)
        {
            parent.Validate(commandResult);
        }
    }

    public abstract class GeneratedCommandBase<TSelf, TResult> : GeneratedCommandBase
        where TSelf : GeneratedCommandBase
        where TResult : new()
    {
        protected GeneratedCommandBase(string name, string? description = null)
            : base(name, description) { }

        public abstract TResult GetResult(CommandResult commandResult);

        public TResult GetResult(InvocationContext context) => GetResult(context.ParseResult.CommandResult);
    }

    public abstract class GeneratedCommandBase
    {
        // no op: for generation
        public void AddCommand(Delegate method) { }
        public void AddCommands(params Delegate[] method) { }
        public void AddCommand<TAttachTo>(Delegate method) { }
        public void AddCommands<TAttachTo>(params Delegate[] method) { }
        public virtual void Validate(CommandResult commandResult) { }
        public void AddValidator(Delegate action, params object[] values) { }

        private readonly Command sclCommand;
        protected GeneratedCommandBase(string name, string? description = null)
        {
            sclCommand = new Command(name, description);
        }

        protected void AddMessageOnFail(List<string> messages, string? newMessage)
        {
            if (string.IsNullOrWhiteSpace(newMessage))
            { return; }
            messages.Add(newMessage);
        }

        protected void AddMessagesOnFail(List<string> messages, IEnumerable<string>? newMessages)
        {
            if (newMessages is not null && newMessages.Any())
            { messages.AddRange(newMessages); }
        }

        protected Command SystemCommandLineCommand => sclCommand;

        /// <summary>
        /// Adds a subcommand to the command.
        /// </summary>
        /// <param name="command">The subcommand to add to the command.</param>
        /// <remarks>Commands can be nested to an arbitrary depth.</remarks>
        protected void AddCommandToScl(GeneratedCommandBase command)
            => sclCommand.AddCommand(command.SystemCommandLineCommand);


        internal int Run(string[] args)
        {
            return sclCommand.Invoke(args);
        }

        //*** Put SCL wrappers below. Avoiding regions to avoid antagonizing region haters
        /// <summary>
        /// Gets the set of strings that can be used on the command line to specify the symbol.
        /// </summary>
        public IReadOnlyCollection<string> Aliases => sclCommand.Aliases;

        /// <inheritdoc/>
        public string Name
        {
            get => sclCommand.Name;
            set => sclCommand.Name = value;
        }

        /// <summary>
        /// Determines whether the specified alias has already been defined.
        /// </summary>
        /// <param name="alias">The alias to search for.</param>
        /// <returns><see langword="true" /> if the alias has already been defined; otherwise <see langword="false" />.</returns>
        public bool HasAlias(string alias) => sclCommand.HasAlias(alias);

        /// <summary>
        /// Represents all of the arguments for the command.
        /// </summary>
        public IReadOnlyList<Argument> Arguments => sclCommand.Arguments;

        /// <summary>
        /// Represents all of the options for the command, including global options.
        /// </summary>
        public IReadOnlyList<Option> Options => sclCommand.Options;

        /// <summary>
        /// Represents all of the global options for the command
        /// </summary>
        public IReadOnlyList<Option> GlobalOptions => sclCommand.Options;


        /// <summary>
        /// Adds an alias to the command. Multiple aliases can be added to a command, most often used to provide a
        /// shorthand alternative.
        /// </summary>
        /// <param name="alias">The alias to add to the command.</param>
        public void AddAlias(string alias) => sclCommand.AddAlias(alias);

        /// <summary>
        /// Gets or sets a value that indicates whether unmatched tokens should be treated as errors. For example,
        /// if set to <see langword="true"/> and an extra command or argument is provided, validation will fail.
        /// </summary>
        public bool TreatUnmatchedTokensAsErrors { get; set; } = true;

        /// <summary>
        /// Represents all of the symbols for the command.
        /// </summary>
        public IEnumerator<Symbol> GetEnumerator() => sclCommand.GetEnumerator();

        /// <summary>
        /// Gets completions for the symbol.
        /// </summary>
        public IEnumerable<CompletionItem> GetCompletions(CompletionContext context) => sclCommand.GetCompletions(context);

        /// <summary>
        /// Adds an <see cref="Option"/> to the command.
        /// </summary>
        /// <param name="option">The option to add to the command.</param>
        protected void Add(Option option) => sclCommand.AddOption(option);

        /// <summary>
        /// Adds an <see cref="Argument"/> to the command.
        /// </summary>
        /// <param name="argument">The argument to add to the command.</param>
        protected void Add(Argument argument) => sclCommand.AddArgument(argument);

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
            get => sclCommand.Handler;
            set => sclCommand.Handler = value;
        }



    }

    public class EmptyCommand : GeneratedCommandBase
    {
        public EmptyCommand() : base("<EmptyCommand>", null)
        {
        }
    }
}
