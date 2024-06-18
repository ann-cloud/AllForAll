using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dto.Manufacturer
{
    public class ManufacturerRequestDto
    {
     
        public string? Name { get; set; }

        public string? Country { get; set; }

        public string? Desc { get; set; }

        public string? ManufacturerPhotoLink { get; set; }
    }
}
