namespace DemoHandlers
{
    internal class Validators
    {
        public static IEnumerable<string> FranchiseValidate(string greeting)
        {
            var errors = new List<string>();
            if (greeting.Contains("Poo", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("We do not say 'Poo' on this ship!");
            }
            return errors;
        }
    }
}
