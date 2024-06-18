
using BusinessLogic.Dto.Product;
using BusinessLogic.Implementation;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace AllForAll.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController: ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IPhotoService _photoService;
        public ProductsController(IPhotoService photoService, IProductService productService)
        {
            _photoService = photoService;
            _productService = productService;
        }


        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync (CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllProductsAsync(cancellationToken);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync([FromRoute] int id ,CancellationToken cancellationToken)
        {
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromForm] ProductRequestDto productDto,IFormFile file, CancellationToken cancellationToken)
        {
            if (file != null && file.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(file);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Failed to upload photo");
                }

                productDto.ProductPhotoLink = uploadResult.SecureUrl.AbsoluteUri;
            }

            var productId = await _productService.CreateProductAsync(productDto, cancellationToken);

            if (productId == 0)
            {
                return BadRequest("Failed to create product");
            }

            return Ok(productId);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync([FromRoute]int id, [FromBody]ProductRequestDto productDto, CancellationToken cancellation = default)
        {
            await _productService.UpdateProductAsync(id, productDto, cancellation);
            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProductAsync([FromRoute] int id, CancellationToken cancellation = default)
        {
            await _productService.DeleteProductAsync(id, cancellation);
            return NoContent();
        }

        [HttpPost("upload-photo/{productId}")]
        public async Task<IActionResult> UploadProductPhoto(int productId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("File is empty");
            }
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var uploadResult = await _photoService.AddPhotoAsync(file);
            if (uploadResult.Error != null)
            {
                return BadRequest("Failed to upload photo");
            }

            product.ProductPhotoLink = uploadResult.SecureUrl.AbsoluteUri;

            await _productService.UpdateProductAsync(productId, new ProductRequestDto { ProductPhotoLink = product.ProductPhotoLink });

            return Ok("Product photo uploaded successfully");
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularProducts(CancellationToken cancellationToken)
        {
            var popularProducts = await _productService.GetPopularProductsAsync(cancellationToken);
            return Ok(popularProducts);
        }


    }
}
