using AuthenticationService.Entity;

namespace AuthenticationService.IRepository;

public interface IUserPermissionRepository
{
    Task<UserPermission?> GetUserPermissionByOaUsernameAsync(string oa_username, CancellationToken cancellationToken);
    Task<bool> AddUserPermissionAsync(UserPermission body, CancellationToken cancellationToken);
    Task<bool> UpdateRefreshTokenByUsernameAsync(UserPermission body, CancellationToken cancellationToken);
}