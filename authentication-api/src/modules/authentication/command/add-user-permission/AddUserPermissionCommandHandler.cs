using AuthenticationService.Commands;
using MediatR;
using AuthenticationService.IRepository;

namespace EmployeeService.CommandHandler;

public class AddUserPermissionCommandHandler(IUserPermissionRepository userPermissionRepository) : IRequestHandler<AddUserPermissionCommand, bool>
{
    private readonly IUserPermissionRepository _userPermissionRepository = userPermissionRepository;
    public async Task<bool> Handle(AddUserPermissionCommand request, CancellationToken cancellationToken)
    {
        var result = await _userPermissionRepository.AddUserPermissionAsync(request.AddUserPermissionModel, cancellationToken);
        return result;
    }
}