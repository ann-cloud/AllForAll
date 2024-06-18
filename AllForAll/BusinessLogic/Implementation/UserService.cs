using AllForAll.Models;
using AutoMapper;
using BusinessLogic.Dto.User;
using BusinessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BusinessLogic.Dto.UserRole;
using BusinessLogic.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Implementation
{
    public class UserService : IUserService
    {
        private readonly AllForAllDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly JWTSettings _options;
        private readonly GoogleJwt _googleOptions;
        private readonly IPhotoService _photoService;

        public UserService(AllForAllDbContext dbContext, IMapper mapper, IOptions<JWTSettings> optAccess, IOptions<GoogleJwt> optGoogle, IPhotoService photoService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _options = optAccess.Value;
            _googleOptions = optGoogle.Value;
            _photoService = photoService;

        }
        private string ComputeObjectHash<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                var hashBytes = sha256.ComputeHash(bytes);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public async Task<int> CreateUserAsync(UserRequestDto user, CancellationToken cancellation = default)
        {
            var mappedUser = _mapper.Map<User>(user);
            var createdUser = await _dbContext.Users.AddAsync(mappedUser, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);
            return createdUser.Entity.UserId;
        }

        public async Task DeleteUserAsync(int id, CancellationToken cancellation = default)
        {
            var userToDelete = await _dbContext.Users.FindAsync(id, cancellation);
            if (userToDelete != null)
            {
                _dbContext.Users.Remove(userToDelete);
                await _dbContext.SaveChangesAsync(cancellation);
            }
        }

        public async Task<ICollection<User>> GetAllUsersAsync(CancellationToken cancellation = default)
        {
            return await _dbContext.Users.ToListAsync(cancellation);
        }

        public async Task<User> GetUserByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id, cancellation);
        }

        public async Task<bool> IsUserExistAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbContext.Users.AnyAsync(u => u.UserId == id, cancellation);
        }

        public async Task UpdateUserAsync(int id, UserRequestDto user, CancellationToken cancellation = default)
        {
            var userToUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id, cancellation);
            if (userToUpdate != null)
            {
                _mapper.Map(user, userToUpdate);
                _dbContext.Update(userToUpdate);
                await _dbContext.SaveChangesAsync(cancellation);
            }
        }
        public async Task UpdateUserAsync(User user, CancellationToken cancellation = default)
        {
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync(cancellation);
        }
        public async Task<string> CreateTokenAsync(User user)
        {
            string IdStringUser = user.UserId.ToString();
            string IdStringRole = user.UserRoleId.ToString();

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, IdStringUser));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            claims.Add(new Claim(ClaimTypes.Role, IdStringRole));
            claims.Add(new Claim("PhotoUrl", user.UserPhotoLink));
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(100)),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(jwt);
            return token;
        }

        public async Task<string> LoginAsync(UserLoginRequestDto model)
        {
            string user = null;
            var passwordHash = ComputeObjectHash<string>(model.Password);
            var existinguser = await _dbContext.Users
                .Where(a => a.Password == passwordHash && a.Email == model.Email)
                .FirstOrDefaultAsync();
            if (existinguser != null)
            {
                user = await CreateTokenAsync(existinguser);
                return user;
            }
            else
            {
                return user;
            }
        }
        public async Task<UserTokenDto> CheckToken(string token)
        {
            string secretKey = _options.Secret;
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,

                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience
            };
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var UserId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var Email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var Username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var RoleId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var Photo = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "PhotoUrl")?.Value;
                int Idinteger = int.Parse(UserId);
                UserTokenDto user = new UserTokenDto()
                {
                    UserId = Idinteger,
                    Email = Email,
                    Username = Username,
                    userRoleId = RoleId,
                    UserPhotoLink = Photo
                };

                return (user);
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }
        public async Task<string> LoginGoogle(string token, CancellationToken cancellation = default)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwksUrl = "https://www.googleapis.com/oauth2/v3/certs";
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(jwksUrl);
            var jwks = JsonConvert.DeserializeObject<JsonWebKeySet>(json);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKeys = jwks.Keys,
                ValidIssuer = _googleOptions.Issuer,
                ValidAudience = _googleOptions.Audience
            };
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var Email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var Username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                var Photo = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
                var email_check = await _dbContext.Users
                        .Where(a => a.Email == Email)
                        .FirstOrDefaultAsync();
                if (email_check != null)
                {
                    return await CreateTokenAsync(email_check);
                }
                else
                {
                    var UrlPhoto = await DownloadAndSaveImage(Photo);

                    var newuser = new User
                    {
                        Email = Email,
                        Username = Username,
                        DateJoined = DateOnly.FromDateTime(DateTime.Now.Date),
                        IsGoogleAcc = "Yes",
                        UserRoleId = 2,
                        UserPhotoLink = UrlPhoto
                    };
                    var createdUser = await _dbContext.Users.AddAsync(newuser, cancellation);
                    await _dbContext.SaveChangesAsync(cancellation);
                    var existinguser = await _dbContext.Users
                        .Where(a => a.Email == newuser.Email)
                        .Include(a => a.UserRole)
                        .FirstOrDefaultAsync();
                    return await CreateTokenAsync(existinguser);
                }
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }
        public async Task<string> RegisterAsync(UserRequestDto user)
        {
            var email_check = await _dbContext.Users
                       .Where(a => a.Email == user.Email)
                       .FirstOrDefaultAsync();
            if (email_check != null)
            {
                return "Such user already exists";
            }
            else
            {
                var mappedUser = _mapper.Map<User>(user);
                var passwordHash = ComputeObjectHash<string>(mappedUser.Password);
                mappedUser.Password = passwordHash;
                mappedUser.DateJoined = DateOnly.FromDateTime(DateTime.Now.Date);
                mappedUser.IsGoogleAcc = "No";
                mappedUser.UserPhotoLink = "https://conflictresolutionmn.org/wp-content/uploads/2020/01/flat-business-man-user-profile-avatar-icon-vector-4333097.jpg";
                mappedUser.UserRoleId = 2;
                await _dbContext.Users.AddAsync(mappedUser);
                await _dbContext.SaveChangesAsync();
                return "User Add";
            }
        }
        private async Task<string> DownloadAndSaveImage(string imageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(imageUrl);
                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var imageStream = new MemoryStream(imageBytes);
                    var fileName = Path.GetFileName(imageUrl);
                    var UrlPhoto = await _photoService.AddPhotoAsync(new FormFile(imageStream, 0, imageBytes.Length, "image", fileName));
                    return UrlPhoto.SecureUrl.AbsoluteUri;
                }
                else
                {
                    return "https://res.cloudinary.com/dhn3lh7jg/image/upload/v1714167572/wg3enteapj3semcwc0qg.png";
                }
            }
        }
    }
}
