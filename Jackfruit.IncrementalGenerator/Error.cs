using Microsoft.CodeAnalysis;

namespace Jackfruit.IncrementalGenerator
{


    /// <summary>
    /// This is mostly for internal errors, but in a format that can be reported. Default is not reported and the reported info is likely to be sparse.
    /// </summary>
    public class Error
    {
        public const string NoActionOrSubCommandsId = "JF0001";
        public const string NoActionOrSubCommandsMessage = "Use the AddSubCommand method to add subcommands, or the SetAction to specify an action (or both)";
        public const string SetDelegateNotMethodGroupId = "JF0002";
        public const string SetDelegateNotMethodGroupMessage = "SetAction should have a single argument that is a method name without the parentheses (this also called a method group)";

        public string? Id { get; set; }
        public bool ReportToUser { get; set; }
        public Location? Location { get; set; }
        public LocalizableString? Title { get; set; }
        public LocalizableString? Message { get; set; }
        public string? Category { get; set; }
        public DiagnosticSeverity? DefaultSeverity { get; set; }
        public bool IsDiagnosticEnabledByDefault { get; set; } = true;
        public LocalizableString? Description { get; set; }
        public string? HelpLinkUri { get; set; }
        public List<string> CustomTag { get; set; } = new();
        public string[] MessageArgs { get; set; } = new string[0];
    }

    public static class ErrorExtensions
    {
        public static CommandDetail UserWarning(this CommandDetail commandDetail,
                                       string id,
                                       string message,
                                       Location location,
                                       params string[] messageaArgs)
        {
             commandDetail.Errors.Add(new Error
            {
                Id = id,
                Message = message,
                MessageArgs = messageaArgs,
                DefaultSeverity = DiagnosticSeverity.Warning,
                ReportToUser = true,
                Location = location
            });
            return commandDetail;
        }

        public static CommandDetail InternalError(this CommandDetail commandDetail,
                                                  string message,
                                                  Location location)
        {
             commandDetail.Errors.Add(new Error
            {
                Message = message,
                DefaultSeverity = DiagnosticSeverity.Error,
                Location = location
            });
            return commandDetail;
        }
    }
}
