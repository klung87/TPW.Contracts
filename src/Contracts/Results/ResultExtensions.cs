namespace Contracts.Results;

public static class ResultExtensions
{
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> result, Func<TIn, Task<TOut>> onValue) =>
        await (await result).BindAsync(onValue);
}
