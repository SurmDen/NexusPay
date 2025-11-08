namespace Transaction.Domain.Exceptions
{
    public class NotFountException : Exception
    {
        public NotFountException( string message) : base(message ) { }
    }
}
