namespace Jackfruit;

public class Validators
{
    public static string ValidatePoliteness(string value)
    {
        if (value.Contains( "please", StringComparison.OrdinalIgnoreCase))
        { return null; }
        return "We are polite on this ship, you hooligan!";
    }


}
