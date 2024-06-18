using AllForAll.Models;
using AutoMapper;
using BusinessLogic.Dto.UserRole;
using BusinessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic.Implementation
{
    public class UserRoleService : IUserRoleService
    {
        private readonly AllForAllDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserRoleService(AllForAllDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> CreateUserRoleAsync(UserRoleRequestDto userRole, CancellationToken cancellation = default)
        {
            var mappedUserRole = _mapper.Map<UserRole>(userRole);
            var createdUserRole = await _dbContext.UserRoles.AddAsync(mappedUserRole, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);
            return createdUserRole.Entity.UserRoleId;
        }

        public async Task DeleteUserRoleAsync(int id, CancellationToken cancellation = default)
        {
            var userRoleToDelete = await _dbContext.UserRoles.FindAsync(id, cancellation);
            if (userRoleToDelete != null)
            {
                _dbContext.UserRoles.Remove(userRoleToDelete);
                await _dbContext.SaveChangesAsync(cancellation);
            }
        }

        public async Task<ICollection<UserRole>> GetAllUserRolesAsync(CancellationToken cancellation = default)
        {
            return await _dbContext.UserRoles.ToListAsync(cancellation);
        }

        public async Task<UserRole> GetUserRoleByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserRoleId == id, cancellation);
        }

        public async Task<bool> IsUserRoleExistAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbContext.UserRoles.AnyAsync(ur => ur.UserRoleId == id, cancellation);
        }

        public async Task UpdateUserRoleAsync(int id, UserRoleRequestDto userRole, CancellationToken cancellation = default)
        {
            var userRoleToUpdate = await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserRoleId == id, cancellation);
            if (userRoleToUpdate != null)
            {
                _mapper.Map(userRole, userRoleToUpdate);
                _dbContext.Update(userRoleToUpdate);
                await _dbContext.SaveChangesAsync(cancellation);
            }
        }
    }
}
