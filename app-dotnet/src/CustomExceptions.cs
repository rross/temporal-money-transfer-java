namespace MoneyTransfer;

public class InvalidAccountException : Exception
{
    public InvalidAccountException(string message) : base(message)
    {            
    }
}
