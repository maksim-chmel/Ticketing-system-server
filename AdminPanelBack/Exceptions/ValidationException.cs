namespace AdminPanelBack.Exceptions;

public sealed class ValidationException(string detail)
    : HttpException(StatusCodes.Status400BadRequest, "Bad Request", detail);

