using MediatR;
using AuthenticationService.Entity;

namespace AuthenticationService.Commands;

public record UpdateRefreshTokenByUsernameCommand(UserPermission AddUserPermissionModel) : IRequest<bool>;
