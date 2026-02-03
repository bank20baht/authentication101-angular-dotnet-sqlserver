using AuthenticationService.Commands;
using AuthenticationService.IRepository;
using MediatR;
using Shared.ApplicationErrors;

namespace AuthenticationService.CommandHandler;

public class UpdateRefreshTokenByUsernameCommandHandler(IUserPermissionRepository userPermissionRepository) : IRequestHandler<UpdateRefreshTokenByUsernameCommand, bool>
{
    private readonly IUserPermissionRepository _userPermissionRepository = userPermissionRepository;
    public async Task<bool> Handle(UpdateRefreshTokenByUsernameCommand request, CancellationToken cancellationToken)
    {
        var result = await _userPermissionRepository.UpdateRefreshTokenByUsernameAsync(request.AddUserPermissionModel, cancellationToken);
        if (!result)
        {
            throw new DatabaseUpdateStatusErrorException("can not update refresh token");
        }
        return result;
    }
}