using System;
using System.Collections.Generic;

namespace AllForAll.Models;

public partial class Manufacturer
{
    public int ManufacturerId { get; set; }

    public string? Name { get; set; }

    public string? Country { get; set; }

    public string? Desc { get; set; }

    public string? ManufacturerPhotoLink { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
