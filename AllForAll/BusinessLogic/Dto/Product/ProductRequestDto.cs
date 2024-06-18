namespace BusinessLogic.Dto.Product
{
    public class ProductRequestDto
    {
   
        public string? ProductName { get; set; }

        public int? CategoryId { get; set; }

        public int? ManufacturerId { get; set; }

        public string? CreationDate { get; set; }

        public string? ProductPhotoLink { get; set; }

        public bool? IsVerified { get; set; }

        public string? Country { get; set; }
    }
}
