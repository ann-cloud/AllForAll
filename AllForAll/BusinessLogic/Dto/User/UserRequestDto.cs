using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dto.User
{
    public class UserRequestDto
    {
        public string? Username { get; set; }

        public string? Email { get; set; }

        public int? UserRoleId { get; set; }

        public DateOnly? DateJoined { get; set; }

        public string? Password { get; set; }

        public string? IsGoogleAcc { get; set; }

        public string? UserPhotoLink { get; set; }
    }
}
