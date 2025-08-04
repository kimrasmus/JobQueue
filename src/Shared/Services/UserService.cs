using Shared.DTOs;
using Shared.Interfaces;

namespace Shared.Services;

public class UserService : IUserService
{
    public UserDto GetLoggedInUser()
    {
        return new UserDto();
    }
}
