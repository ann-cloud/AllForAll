
using BusinessLogic.Dto.User;
using BusinessLogic.Implementation;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AllForAll.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace AllForAll.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public UsersController(IPhotoService photoService, IUserService userService)
        {
            _photoService = photoService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        #region Get user`s param
        [HttpGet("GetUserByIdAsync/{id}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        //Get user`s UserRole
        [HttpGet("GetUserRole/{id}")]
        public async Task<IActionResult> GetUserRole([FromBody] int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user != null)
            {
                return Ok(user.UserRole);
            }

            return NotFound();
        }
        
        //Get user`s username
        [HttpGet("GetUsername/{id}")]
        public async Task<IActionResult> GetUsername([FromBody] int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user != null)
            {
                return Ok(user.Username);
            }
            return NotFound();
        }
        
        //Get user`s email
        [HttpGet("GetEmail/{id}")]
        public async Task<IActionResult> GetEmail([FromBody] int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user != null)
            {
                return Ok(user.Email);
            }
            return NotFound();
        }
        
        //Get user`s Is Google account
        [HttpGet("GetIsGoogleAcc/{id}")]
        public async Task<IActionResult> GetIsGoogleAcc([FromBody] int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user != null)
            {
                return Ok(user.IsGoogleAcc);
            }
            return NotFound();
        }
        
        //Get user`s feedbacks
        [HttpGet("GetUserFeedbacks/{id}")]
        public async Task<IActionResult> GetUserFeedbacks([FromBody] int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user != null)
            {
                return Ok(user.Feedbacks);
            }
            return NotFound();
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromForm] UserRequestDto userDto, CancellationToken cancellationToken)
        {


            var userId = await _userService.CreateUserAsync(userDto, cancellationToken);


            return Ok(userId);
        }

        #region Update user`s param
        [HttpPut("UpdateUserAsync/{id}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] int id, [FromBody] UserRequestDto user, CancellationToken cancellationToken)
        {
            await _userService.UpdateUserAsync(id, user, cancellationToken);
            return NoContent();
        }

        //Update user`s role
        [HttpPut("UpdateUserRoleAsync/{id}")]
        public async Task<IActionResult> UpdateUserRoleAsync([FromRoute] int id, [FromBody] UserRole userRole,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            user.UserRole = userRole;
            await _userService.UpdateUserAsync(user, cancellationToken);
            return NoContent();
        }


        //Update user`s email
        [HttpPut("UpdateUserEmailAsync/{id}")]
        public async Task<IActionResult> UpdateUserEmailAsync([FromBody] int id, string email,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            user.Email = email;
            await _userService.UpdateUserAsync(user, cancellationToken);
            return NoContent();
        }
        
        //Update username
        [HttpPut("UpdateUsernameAsync/{id}")]
        public async Task<IActionResult> UpdateUsernameAsync([FromBody] int id, string username,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            user.Username = username;
            await _userService.UpdateUserAsync(user, cancellationToken);
            return NoContent();
        }
        
        //Update user`s is google account
        [HttpPut("UpdateIsGoogleAccAsync/{id}")]
        public async Task<IActionResult> UpdateIsGoogleAccAsync([FromBody] int id, string isGoogleAcc,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            user.IsGoogleAcc = isGoogleAcc;
            await _userService.UpdateUserAsync(user, cancellationToken);
            return NoContent();
        }
        #endregion

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _userService.DeleteUserAsync(id, cancellationToken);
            return NoContent();
        }
        [HttpPost("upload-photo/{userId}")]
        public async Task<IActionResult> UploadPhoto(int userId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("File is empty");
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var uploadResult = await _photoService.AddPhotoAsync(file);
            if (uploadResult.Error != null)
            {
                return BadRequest("Failed to upload photo");
            }

            user.UserPhotoLink = uploadResult.SecureUrl.AbsoluteUri;

            await _userService.UpdateUserAsync(userId, new UserRequestDto { UserPhotoLink = user.UserPhotoLink });

            return Ok("Photo uploaded successfully");
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginAsync([FromForm] UserLoginRequestDto user)
        {
            var token = await _userService.LoginAsync(user);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("CheckToken")]
        public async Task<IActionResult> CheckToken()
        {
            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { Message = "Invalid token format or token missing" });
            }
            string token = authorizationHeader.Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(token) || token == "null")
            {
                return BadRequest(new { Message = "The token does not exist or is null" });
            }
            var user = await _userService.CheckToken(token);
            return Ok(user);
        }
        [HttpPost("LoginGoogle")]
        public async Task<IActionResult> LoginGoogle([FromBody] string credential, CancellationToken cancellationToken)
        {
            var token = await _userService.LoginGoogle(credential, cancellationToken);
            return Ok(new { token });
        }
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromForm] UserRequestDto userdto, CancellationToken cancellationToken)
        {
            var message = await _userService.RegisterAsync(userdto);
            return Ok(new { message });
        }

    }
}
