using System.Diagnostics.CodeAnalysis;
using Shared.Dto;

namespace Shared.ApplicationErrors
{
    [ExcludeFromCodeCoverage]
    public class NotFoundException(string message) : Exception(message)
    {
    }
    [ExcludeFromCodeCoverage]
    public class InvalidPassword(string message) : Exception(message)
    {
    }

    [ExcludeFromCodeCoverage]
    public class ApplicationErrorException(string message) : Exception(message)
    {

    }

    [ExcludeFromCodeCoverage]
    public class DuplicateRecordErrorException(string message) : Exception(message)
    {

    }

    [ExcludeFromCodeCoverage]
    public class CannotUpdateStockErrorException(string message) : Exception(message)
    {

    }

    [ExcludeFromCodeCoverage]
    public class NotEnoughProduct(string message) : Exception(message)
    {

    }

    [ExcludeFromCodeCoverage]
    public class DatabaseUpdateStatusErrorException(string message) : Exception(message)
    {

    }


    [ExcludeFromCodeCoverage]
    public class NotExistingUserErrorException(string message) : Exception(message)
    {

    }

    [ExcludeFromCodeCoverage]
    public class RefreshTokenNotSameErrorException(string message) : Exception(message)
    {

    }
}



