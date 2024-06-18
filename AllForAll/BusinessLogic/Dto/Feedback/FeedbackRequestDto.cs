using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dto.Feedback
{
    public class FeedbackRequestDto
    {
        public int? ProductId { get; set; }

        public int? UserId { get; set; }

        public decimal? Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime? FeedbackDate { get; set; }
    }
}
