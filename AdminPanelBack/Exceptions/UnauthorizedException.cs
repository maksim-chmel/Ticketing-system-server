namespace AdminPanelBack.Exceptions;

public sealed class UnauthorizedException(string detail)
    : HttpException(StatusCodes.Status401Unauthorized, "Unauthorized", detail);

