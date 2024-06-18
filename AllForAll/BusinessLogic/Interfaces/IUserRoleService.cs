using AllForAll.Models;
using BusinessLogic.Dto.User;
using BusinessLogic.Dto.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserRoleService
    {
        Task<ICollection<UserRole>> GetAllUserRolesAsync(CancellationToken cancellation = default);
        Task<UserRole> GetUserRoleByIdAsync(int id, CancellationToken cancellation = default);

        Task<bool> IsUserRoleExistAsync(int id, CancellationToken cancellation = default);

        Task<int> CreateUserRoleAsync(UserRoleRequestDto userRole, CancellationToken cancellation = default);

        Task UpdateUserRoleAsync(int id, UserRoleRequestDto userRole, CancellationToken cancellation = default);

        Task DeleteUserRoleAsync(int id, CancellationToken cancellation = default);
    }
}
