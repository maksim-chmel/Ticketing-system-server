namespace AdminPanelBack.Exceptions;

public sealed class NotFoundException(string detail)
    : HttpException(StatusCodes.Status404NotFound, "Not Found", detail);

