using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dto.Category
{
    public class CategoryRequestDto
    {
        public string? Name { get; set; }

        public string? Desc { get; set; }

        public string? CategoryPhotoLink { get; set; }
    }
}
