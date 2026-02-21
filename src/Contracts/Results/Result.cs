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

    public Result(Error error)
    {
        _isSuccess = false;
        _errors = [error];
    }

    public Result(IEnumerable<Error> errors)
    {
        if (!errors.Any())
        {
            throw new ArgumentException("Errors cannot be empty");
        }

        _errors = errors;
        _value = default;
        _isSuccess = false;
    }

    public Result<TOut> Bind<TOut>(Func<T, TOut> onValue) =>
        IsSuccess ? new Result<TOut>(onValue(_value!)) : new Result<TOut>(_errors);

    public Result<T> Effect(Action<T> onValue)
    {
        if (IsSuccess)
        {
            onValue(_value!);
        }

        return this;
    }

    public async Task<Result<TOut>> BindAsync<TOut>(Func<T, Task<TOut>> onValueAsync)
    {
        if (IsSuccess)
        {
            try
            {
                var value = await onValueAsync(_value!);

                return value is null
                    ? new Result<TOut>(Error.NullValue(typeof(TOut)))
                    : new Result<TOut>(value);
            }
            catch (Exception ex) when (!IsFatal(ex))
            {
                return new Result<TOut>(Error.FromException(ex));
            }
        }

        return new Result<TOut>(_errors);
    }

    private T GetValueOrThrow() => _isSuccess ? _value! : throw new InvalidOperationException("Value of result is null");

    public static implicit operator Result<T>(T item) => new(item);
    public static implicit operator Result<T>(Error error) => new([error]);

    public static explicit operator T(Result<T> result) => result.GetValueOrThrow();

    private static bool IsFatal(Exception ex) =>
        ex is OperationCanceledException
        or StackOverflowException
        or AccessViolationException
        or OutOfMemoryException
        or BadImageFormatException
        or ThreadAbortException;
}
