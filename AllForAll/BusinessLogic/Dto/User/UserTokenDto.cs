using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dto.User
{
    public class UserTokenDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserPhotoLink { get; set; }
        public string userRoleId { get; set; }



    }
}
