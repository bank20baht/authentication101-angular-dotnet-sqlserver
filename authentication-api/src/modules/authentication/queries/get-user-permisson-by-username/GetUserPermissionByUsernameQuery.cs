using MediatR;
using AuthenticationService.Entity;

namespace AuthenticationService.Query;

public record GetUserPermissionByUsernameQuery(string OaUsername) : IRequest<UserPermission?>;