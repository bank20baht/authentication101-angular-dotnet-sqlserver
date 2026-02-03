using MediatR;
using AuthenticationService.Entity;
using AuthenticationService.IRepository;
using AuthenticationService.Query;

namespace AuthenticationService.QueryHandler;

public class GetUserPermissionByUsernameQueryHandler(IUserPermissionRepository userPermissionRepository) : IRequestHandler<GetUserPermissionByUsernameQuery, UserPermission?>
{
    private readonly IUserPermissionRepository _userPermissionRepository = userPermissionRepository;

    public async Task<UserPermission?> Handle(GetUserPermissionByUsernameQuery request, CancellationToken cancellationToken)
    {
        var result = await _userPermissionRepository.GetUserPermissionByOaUsernameAsync(request.OaUsername, cancellationToken);
        return result;
    }
}