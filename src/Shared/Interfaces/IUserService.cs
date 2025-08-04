using Shared.DTOs;
using Shared.Entities;

namespace Shared.Interfaces;

public interface IUserService
{
    UserDto GetLoggedInUser();
}
