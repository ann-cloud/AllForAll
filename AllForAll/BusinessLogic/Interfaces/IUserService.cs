using AllForAll.Models;
using BusinessLogic.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<User>> GetAllUsersAsync(CancellationToken cancellation = default);
        Task<User> GetUserByIdAsync(int id, CancellationToken cancellation = default);

        Task<bool> IsUserExistAsync(int id, CancellationToken cancellation = default);

        Task<int> CreateUserAsync(UserRequestDto user, CancellationToken cancellation = default);

        Task UpdateUserAsync(int id, UserRequestDto user, CancellationToken cancellation = default);
        Task UpdateUserAsync(User user, CancellationToken cancellation = default);
        Task<string> CreateTokenAsync(User user);
        Task<UserTokenDto> CheckToken(string token);

        Task<string> LoginAsync(UserLoginRequestDto model);
        Task<string> RegisterAsync(UserRequestDto user);
        Task<string> LoginGoogle(string token, CancellationToken cancellation = default);

        Task DeleteUserAsync(int id, CancellationToken cancellation = default);

    }
}
