using AutoMapper;
using stokTakip.DTOs;
using stokTakip.Models;

namespace stokTakip.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Customer mappings
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();

            // Supplier mappings
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>();
            CreateMap<UpdateSupplierDto, Supplier>();

            // Sale mappings
            CreateMap<Sale, SaleDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName));
            CreateMap<SaleItem, SaleItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
            CreateMap<CreateSaleDto, Sale>();
            CreateMap<CreateSaleItemDto, SaleItem>();
            CreateMap<UpdateSaleDto, Sale>();

            // Purchase mappings
            CreateMap<Purchase, PurchaseDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.SupplierName));
            CreateMap<PurchaseItem, PurchaseItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
            CreateMap<CreatePurchaseDto, Purchase>();
            CreateMap<CreatePurchaseItemDto, PurchaseItem>();
            CreateMap<UpdatePurchaseDto, Purchase>();

            // StockMovement mappings
            CreateMap<StockMovement, StockMovementDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.MovementTypeName, opt => opt.MapFrom(src => GetMovementTypeName(src.MovementType)));
            CreateMap<CreateStockMovementDto, StockMovement>();
        }

        private static string GetMovementTypeName(Models.StockMovementType movementType)
        {
            return movementType switch
            {
                Models.StockMovementType.In => "Giriş",
                Models.StockMovementType.Out => "Çıkış",
                Models.StockMovementType.Transfer => "Transfer",
                Models.StockMovementType.Adjustment => "Düzeltme",
                _ => "Bilinmeyen"
            };
        }
    }
}
