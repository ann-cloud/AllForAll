using BusinessLogic.Dto.Manufacturer;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AllForAll.Controllers
{
    [ApiController]
    [Route("api/manufacturers")]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly IPhotoService _photoService;
        public ManufacturersController(IPhotoService photoService, IManufacturerService manufacturerService)
        {
            _photoService = photoService;

            _manufacturerService = manufacturerService;
        }



        [HttpGet]
        public async Task<IActionResult> GetAllManufacturersAsync(CancellationToken cancellationToken)
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(cancellationToken);
            return Ok(manufacturers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetManufacturerByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id, cancellationToken);
            if (manufacturer == null)
            {
                return NotFound();
            }
            return Ok(manufacturer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateManufacturerAsync([FromBody] ManufacturerRequestDto manufacturerDto, [FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file != null && file.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(file);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Failed to upload photo");
                }

                manufacturerDto.ManufacturerPhotoLink = uploadResult.SecureUrl.AbsoluteUri;
            }

            var manufacturerId = await _manufacturerService.CreateManufacturerAsync(manufacturerDto, cancellationToken);

            if (manufacturerId == 0)
            {
                return BadRequest("Failed to create manufacturer");
            }

            return Ok(manufacturerId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManufacturerAsync([FromRoute] int id, [FromBody] ManufacturerRequestDto manufacturerDto, CancellationToken cancellationToken)
        {
            await _manufacturerService.UpdateManufacturerAsync(id, manufacturerDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManufacturerAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _manufacturerService.DeleteManufacturerAsync(id, cancellationToken);
            return NoContent();
        }
        [HttpPost("upload-photo/{manufacturerId}")]
        public async Task<IActionResult> UploadManufacturerPhoto(int manufacturerId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("File is empty");
            }
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);
            if (manufacturer == null)
            {
                return NotFound("Manufacturer not found");
            }

            var uploadResult = await _photoService.AddPhotoAsync(file);
            if (uploadResult.Error != null)
            {
                return BadRequest("Failed to upload photo");
            }

            manufacturer.ManufacturerPhotoLink = uploadResult.SecureUrl.AbsoluteUri;

            await _manufacturerService.UpdateManufacturerAsync(manufacturerId, new ManufacturerRequestDto { ManufacturerPhotoLink = manufacturer.ManufacturerPhotoLink });

            return Ok("Manufacturer photo uploaded successfully");
        }
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularManufacturers(CancellationToken cancellationToken)
        {
            var popularManufacturers = await _manufacturerService.GetPopularManufacturersAsync(cancellationToken);
            return Ok(popularManufacturers);
        }

    }
}
