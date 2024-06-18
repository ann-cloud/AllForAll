using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AllForAll.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? CategoryId { get; set; }

    public int? ManufacturerId { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? ProductPhotoLink { get; set; }

    public bool? IsVerified { get; set; }

    public string? Country { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Manufacturer? Manufacturer { get; set; }
}
