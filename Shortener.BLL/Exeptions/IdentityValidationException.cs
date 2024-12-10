namespace Shortener.BLL.Exeptions
{
    public class IdentityValidationException : Exception
    {
        public IdentityValidationException(Dictionary<string, List<string>> errors)
            : base("Identity validation failed")
        {
            Errors = errors ??
                [];
        }

        public IdentityValidationException()
        {
            Errors =
                [];
        }

        public IdentityValidationException(string message)
            : base(message)
        {
            Errors =
                [];
        }

        public IdentityValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Errors =
                [];
        }

        // Замість IEnumerable<string> тепер використовуємо словник
        public Dictionary<string, List<string>> Errors { get; }
    }
}