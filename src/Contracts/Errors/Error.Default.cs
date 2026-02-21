namespace Contracts.Errors;

public sealed partial class Error
{
    public static Error FromException(Exception ex) =>
    new($"Exception: {ex.Message}", -1);

    public static Error NullValue(Type type) =>
        new($"Result value of type {type.Name} was null", -2);
}
