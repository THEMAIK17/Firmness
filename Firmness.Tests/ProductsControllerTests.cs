using AutoMapper;
using Firmness.Api.Controllers;
using Firmness.Api.DTOs.Products;
using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Firmness.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        //  Configure In-Memory Database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        var context = new ApplicationDbContext(options);

        //  Seed test data
        context.Products.Add(new Product { Id = 1, Name = "Ladrillo", UnitPrice = 100, Stock = 50 });
        context.Products.Add(new Product { Id = 2, Name = "Cemento", UnitPrice = 200, Stock = 20 });
        context.SaveChanges();

        // Configure Mapper Mock
        _mockMapper = new Mock<IMapper>();

        // Initialize the Controller
        _controller = new ProductsController(context, _mockMapper.Object);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk_WithListOfDtos()
    {
        // Arrange
        _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<List<Product>>()))
            .Returns(new List<ProductDto> 
            { 
                new ProductDto { Name = "Ladrillo" }, 
                new ProductDto { Name = "Cemento" } 
            });

    
        var result = await _controller.GetProducts();

        // Verify that the response is an ActionResult 
        var actionResult = Assert.IsType<ActionResult<IEnumerable<ProductDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        
        // Verify that it contains the list
        var returnProducts = Assert.IsType<List<ProductDto>>(okResult.Value);
        Assert.Equal(2, returnProducts.Count); 
    }
}