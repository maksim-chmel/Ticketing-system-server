namespace AdminPanelBack.Exceptions;

public sealed class ConflictException(string detail)
    : HttpException(StatusCodes.Status409Conflict, "Conflict", detail);

