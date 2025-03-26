namespace ChessServer.Models;

public struct Result<TSuccess, TError>
{
    public TSuccess? Success { get; }
    public TError? Error { get; }
    
    public bool IsSuccess => Error == null;

    private Result(TSuccess success)
    {
        Success = success;
        Error = default;
    }

    private Result(TError error)
    {
        Success = default;
        Error = error;
    }

    public static Result<TSuccess, TError> Ok(TSuccess value) => new(value);
    public static Result<TSuccess, TError> Fail(TError error) => new(error);
}
