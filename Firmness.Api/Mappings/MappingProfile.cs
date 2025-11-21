using AutoMapper;
using Firmness.Api.DTOs.Clients;
using Firmness.Api.DTOs.Products;
using Firmness.Api.DTOs.Sales;
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
        
        // Entity to Dto Mapping (Output)
        CreateMap<Client, ClientDto>() 
            
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        // DTO to Entity Mapping (Input)
        CreateMap<CreateClientDto, Client>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)); // the username is email

        // DTO to Entity Mapping (Input)
        CreateMap<UpdateClientDto, Client>();
        
        //  Entity to Dto Mapping (Output)
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.FullName)); 

        CreateMap<SaleDetail, SaleDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        // Dto to Entity Mapping (Input)
        CreateMap<CreateSaleDto, Sale>();
        
    }
}