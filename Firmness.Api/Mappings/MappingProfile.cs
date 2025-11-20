using AutoMapper;
using Firmness.Api.DTOs.Products;
using Firmness.Domain.Entities;

namespace Firmness.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity to DTO Mapping (Output)
        CreateMap<Product, ProductDto>();

        // DTO to Entity Mapping (Input)
        CreateMap<CreateProductDto, Product>();
            
        
    }
}