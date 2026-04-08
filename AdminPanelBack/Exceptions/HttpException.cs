namespace AdminPanelBack.Exceptions;

public abstract class HttpException(
    int statusCode,
    string title,
    string detail)
    : Exception(detail)
{
    public int StatusCode { get; } = statusCode;
    public string Title { get; } = title;
    public string Detail { get; } = detail;
}

