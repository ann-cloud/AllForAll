using BusinessLogic.Dto.UserRole;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace AllForAll.Controllers
{
    [ApiController]
    [Route("api/userroles")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserRolesAsync(CancellationToken cancellationToken)
        {
            var userRoles = await _userRoleService.GetAllUserRolesAsync(cancellationToken);
            return Ok(userRoles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRoleByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var userRole = await _userRoleService.GetUserRoleByIdAsync(id, cancellationToken);
            if (userRole == null)
            {
                return NotFound();
            }
            return Ok(userRole);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserRoleAsync([FromBody] UserRoleRequestDto userRoleDto, CancellationToken cancellationToken)
        {
            var userRoleId = await _userRoleService.CreateUserRoleAsync(userRoleDto, cancellationToken);
            return Ok(userRoleId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserRoleAsync([FromRoute] int id, [FromBody] UserRoleRequestDto userRoleDto, CancellationToken cancellationToken)
        {
            await _userRoleService.UpdateUserRoleAsync(id, userRoleDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRoleAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _userRoleService.DeleteUserRoleAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
