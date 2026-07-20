namespace HighFidelity.Ui.Models;

public class Result<T>
{
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public bool IsSuccess => ErrorMessage is null;
    public bool IsFailure => !IsSuccess;

    private Result(T data) => Data = data;
    private Result(string error) => ErrorMessage = error;

    public static Result<T> Success(T data) => new(data);
    public static Result<T> Failure(string error) => new(error);
}
