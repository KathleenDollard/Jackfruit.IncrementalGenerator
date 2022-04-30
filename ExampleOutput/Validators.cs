namespace Jackfruit;

public class Validators
{
    public static string ValidatePoliteness(string value) 
        => value.Contains("junk", StringComparison.OrdinalIgnoreCase)
            ? "We do not say junk on this ship, you hooligan!"
            : "";


}
