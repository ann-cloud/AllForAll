using System;
using System.Collections.Generic;

namespace AllForAll.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? Name { get; set; }

    public string? Desc { get; set; }

    public string? CategoryPhotoLink { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
