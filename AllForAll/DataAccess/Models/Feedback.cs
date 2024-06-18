using System;
using System.Collections.Generic;

namespace AllForAll.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public decimal? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
