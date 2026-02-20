using Contracts.Errors;

namespace Contracts.Results;

/// <summary>
/// A simple Result wrapper that can be used around any type.
/// Will execute given delegates if it holds a value, and propagate an error if it doesn't.
/// Null is not allowed.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Result<T>
{
    private T? _value;
    private IEnumerable<Error> _errors = [];
    private readonly bool _isSuccess;

    public bool IsSuccess => _isSuccess;

    public Result(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException("Null is not a valid Result value");
        }

        _value = item;
        _isSuccess = true;
    }

    public Result(IEnumerable<Error> errors)
    {
        _errors = errors;
        _value = default;
        _isSuccess = false;
    }

    private T GetValue() => _isSuccess ? _value! : throw new InvalidOperationException("Value of result is null");

    public static implicit operator Result<T>(T item) => new(item);
    public static implicit operator Result<T>(Error error) => new([error]);

    public static explicit operator T(Result<T> result) => result.GetValue();
}
