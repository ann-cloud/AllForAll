using AllForAll.Controllers;
using BusinessLogic.Dto.Product;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestProject1;

using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IPhotoService> _mockPhotoService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockPhotoService = new Mock<IPhotoService>();
        _controller = new ProductsController(_mockPhotoService.Object, _mockProductService.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var products = new List<ProductResponseDto> { new ProductResponseDto(), new ProductResponseDto() };
        _mockProductService.Setup(service => service.GetAllProductsAsync(cancellationToken)).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAllProductsAsync(cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<ProductResponseDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetProductByIdAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _mockProductService.Setup(service => service.GetProductByIdAsync(1, cancellationToken)).ReturnsAsync((ProductResponseDto)null);

        // Act
        var result = await _controller.GetProductByIdAsync(1, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProductByIdAsync_ReturnsOkResult_WithProduct()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var product = new ProductResponseDto { Id = 1 };
        _mockProductService.Setup(service => service.GetProductByIdAsync(1, cancellationToken)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetProductByIdAsync(1, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ProductResponseDto>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateProductAsync_ReturnsBadRequest_WhenPhotoUploadFails()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var productDto = new ProductRequestDto();
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.Length).Returns(1);
        _mockPhotoService.Setup(service => service.AddPhotoAsync(fileMock.Object)).ReturnsAsync(new PhotoUploadResult { Error = "Error" });

        // Act
        var result = await _controller.CreateProductAsync(productDto, fileMock.Object, cancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to upload photo", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateProductAsync_ReturnsOkResult_WithProductId()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var productDto = new ProductRequestDto();
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.Length).Returns(1);
        _mockPhotoService.Setup(service => service.AddPhotoAsync(fileMock.Object)).ReturnsAsync(new PhotoUploadResult { SecureUrl = new System.Uri("http://test.com/photo.jpg") });
        _mockProductService.Setup(service => service.CreateProductAsync(productDto, cancellationToken)).ReturnsAsync(1);

        // Act
        var result = await _controller.CreateProductAsync(productDto, fileMock.Object, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, okResult.Value);
    }

    [Fact]
    public async Task UpdateProductAsync_ReturnsNoContent()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var productDto = new ProductRequestDto();

        // Act
        var result = await _controller.UpdateProductAsync(1, productDto, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProductAsync_ReturnsNoContent()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.DeleteProductAsync(1, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UploadProductPhoto_ReturnsBadRequest_WhenFileIsEmpty()
    {
        // Act
        var result = await _controller.UploadProductPhoto(1, null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File is empty", badRequestResult.Value);
    }

    [Fact]
    public async Task UploadProductPhoto_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync((ProductResponseDto)null);

        // Act
        var result = await _controller.UploadProductPhoto(1, new Mock<IFormFile>().Object);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UploadProductPhoto_ReturnsOk_WhenPhotoUploadedSuccessfully()
    {
        // Arrange
        var product = new ProductResponseDto { Id = 1 };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.Length).Returns(1);
        _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync(product);
        _mockPhotoService.Setup(service => service.AddPhotoAsync(fileMock.Object)).ReturnsAsync(new PhotoUploadResult { SecureUrl = new System.Uri("http://test.com/photo.jpg") });

        // Act
        var result = await _controller.UploadProductPhoto(1, fileMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Product photo uploaded successfully", okResult.Value);
    }

    [Fact]
    public async Task GetPopularProducts_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var popularProducts = new List<ProductResponseDto> { new ProductResponseDto(), new ProductResponseDto() };
        _mockProductService.Setup(service => service.GetPopularProductsAsync(cancellationToken)).ReturnsAsync(popularProducts);

        // Act
        var result = await _controller.GetPopularProducts(cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<ProductResponseDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }
}
