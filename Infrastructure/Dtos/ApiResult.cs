using System;

namespace Infrastructure.Dtos;

public class ApiResult<T>
{
    public bool Succeeded { get; set; }
    public string? Error { get; set; }
    public T? Data { get; set; }

    public static ApiResult<T> Ok(T data) => new() { Succeeded = true, Data = data };
    public static ApiResult<T> Fail(string error) => new() { Succeeded = false, Error = error };
}
