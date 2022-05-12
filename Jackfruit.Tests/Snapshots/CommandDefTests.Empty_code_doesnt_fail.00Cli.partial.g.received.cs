//HintName: Cli.partial.g.cs

   namespace Jackfruit
    {
        /// <summary>
        /// This is the entry point for the Jackfruit generator. At present it 'jumps namespaces' 
        /// after first use, moving from Jackfruit to the namespace of your root handler. After 
        /// generation, it will include a static property to access your root by name.
        /// </summary>
        public partial class Cli
        {
            /// <summary>
            /// This method builds a tree that defines your CLI.  
            /// </summary>
            /// <param name="cliRoot">A CliNode pointing to your root handler.</param>
            public static void Create(CliNode cliRoot)
            { }
        }
    }
