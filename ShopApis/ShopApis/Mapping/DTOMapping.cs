// Mapping/DTOMapping.cs
using AutoMapper;
using ShopApis.DTOs;
using ShopApis.Models;

namespace ShopApi.Mapping
{
    public class DTOMapping : Profile
    {

        public DTOMapping()
        {
            // ---------- Category ----------
            CreateMap<Category, CategoryResponseDto>()
                .ForMember(d => d.ProductCount,
                           o => o.MapFrom(s => s.Products != null ? s.Products.Count : 0));
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();

            // ---------- Product ----------
            CreateMap<Product, ProductResponseDto>()
                .ForMember(d => d.CategoryName,
                           o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();

            // ---------- Order ----------
            CreateMap<Order, OrderResponseDto>()
                .ForMember(d => d.ProductName,
                           o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
                .ForMember(d => d.CategoryName,
                           o => o.MapFrom(s => s.Product != null && s.Product.Category != null
                                               ? s.Product.Category.Name : string.Empty));

            // Server-controlled fields are ignored; the service sets them
            CreateMap<OrderCreateDto, Order>()
                .ForMember(d => d.TotalAmount, o => o.Ignore())
                .ForMember(d => d.OrderDate, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.Product, o => o.Ignore());

            CreateMap<OrderUpdateDto, Order>();   // maps only Status
        }


        //public DTOMapping()
        //{
        //    // ---------- Category ----------
        //    CreateMap<Category, CategoryCreateDto>().ReverseMap();
        //    CreateMap<Category, CategoryUpdateDto>().ReverseMap();
        //    CreateMap<Category, CategoryResponseDto>()
        //        .ForMember(d => d.ProductCount,
        //                   o => o.MapFrom(s => s.Products != null ? s.Products.Count : 0));

        //    // ---------- Product ----------
        //    CreateMap<Product, ProductCreateDto>().ReverseMap();
        //    CreateMap<Product, ProductUpdateDto>().ReverseMap();
        //    CreateMap<Product, ProductResponseDto>()
        //        .ForMember(d => d.CategoryName,
        //                   o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

        //    // ---------- Order ----------
        //    CreateMap<Order, OrderCreateDto>().ReverseMap();
        //    CreateMap<Order, OrderUpdateDto>().ReverseMap();
        //    CreateMap<Order, OrderResponseDto>()
        //        .ForMember(d => d.ProductName,
        //                   o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
        //        .ForMember(d => d.CategoryName,
        //                   o => o.MapFrom(s => s.Product != null && s.Product.Category != null
        //                                       ? s.Product.Category.Name : string.Empty));
        //}
    }
}