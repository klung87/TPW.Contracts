namespace Contracts.Errors;

public sealed partial class Error
{
    private Exception? _exception;
    private string _message;
    private int _code;

    public Error(string message, int code, Exception? exception = null)
    {
        _message = message;
        _code = code;
        _exception = exception;
    }
}
