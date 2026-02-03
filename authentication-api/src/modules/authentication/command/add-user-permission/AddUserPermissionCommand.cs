using MediatR;
using AuthenticationService.Entity;

namespace AuthenticationService.Commands;

public record AddUserPermissionCommand(UserPermission AddUserPermissionModel) : IRequest<bool>;