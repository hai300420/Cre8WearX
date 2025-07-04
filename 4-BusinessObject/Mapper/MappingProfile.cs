using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessObject.Enum;
using BusinessObject.Model;
using static BusinessObject.RequestDTO.RequestDTO;

using static BusinessObject.ResponseDTO.ResponseDTO;

namespace BusinessObject.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, LoginResponse>().ReverseMap();
            CreateMap<User, UserListDTO>()
           .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
           .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .IgnoreAllPropertiesWithAnInaccessibleSetter();

            // Product
            CreateMap<Product, ProductListDTO>();
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>();

            // Feedback
            CreateMap<FeedbackDTO, Feedback>().ReverseMap();
            CreateMap<Category, RequestDTO.RequestDTO.CategoryListDTO>();
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<Category, CategoryDetailDTO>();

            // DesignElement Mapping
            CreateMap<DesignElement, DesignElementDTO>()
                .ForMember(dest => dest.DesignElementId, opt => opt.MapFrom(src => src.DesignElementId))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.ColorArea, opt => opt.MapFrom(src => src.ColorArea))
                .ForMember(dest => dest.DesignAreaId, opt => opt.MapFrom(src => src.DesignAreaId))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.DesignArea.AreaName))
                .ForMember(dest => dest.CustomizeProductId, opt => opt.MapFrom(src => src.CustomizeProductId))
                .ForMember(dest => dest.ShirtColor, opt => opt.MapFrom(src => src.CustomizeProduct.ShirtColor))
                .ForMember(dest => dest.FullImage, opt => opt.MapFrom(src => src.CustomizeProduct.FullImage))
                .ReverseMap();
            // Mapping DesignElementCreateDTO <-> DesignElement
            CreateMap<DesignElementCreateDTO, DesignElement>()
                .ForMember(dest => dest.Image, opt => opt.Ignore()) // Image sẽ xử lý riêng (upload file)
                .ForMember(dest => dest.DesignElementId, opt => opt.Ignore()) // ID sẽ được tạo tự động
                .ForMember(dest => dest.DesignArea, opt => opt.Ignore()) // Không map trực tiếp entity liên quan
                .ForMember(dest => dest.CustomizeProduct, opt => opt.Ignore());

            // Add this to your MappingProfile class
            // More specific mapping for when we have product data
            CreateMap<(CreateCustomizeWithOrderDto dto, Product product), CustomizeProduct>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.dto.ProductId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.dto.UserId))
                .ForMember(dest => dest.ShirtColor, opt => opt.MapFrom(src => src.dto.ShirtColor))
                .ForMember(dest => dest.FullImage, opt => opt.MapFrom(src => src.product.Image))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dto.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.product.Price))
                .ForMember(dest => dest.DesignMetadata, opt => opt.MapFrom(src => src.dto.DesignMetadata))
                .ForMember(dest => dest.Base64Image, opt => opt.MapFrom(src => src.dto.Base64Image));



            CreateMap<CustomizeProductResponseDTO, CustomizeProduct>()
                .ForMember(dest => dest.CustomizeProductId, opt => opt.MapFrom(src => src.CustomizeProductId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ShirtColor, opt => opt.MapFrom(src => src.ShirtColor))
                .ForMember(dest => dest.FullImage, opt => opt.MapFrom(src => src.FullImage))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ReverseMap();



            //Oder
            CreateMap<Order, OrderListDTO>().ReverseMap();
            CreateMap<OrderCreateDTO, Order>().ReverseMap();
            CreateMap<OrderUpdateDTO, Order>().ReverseMap();

            // OrderStage
            CreateMap<OrderStage, OrderStageListDTO>().ReverseMap();
            CreateMap<OrderStageCreateDTO, OrderStage>().ReverseMap();


            // Map từ OrderStageCreateDTO sang Entity
            CreateMap<OrderStageCreateDTO, OrderStage>().ReverseMap();

            CreateMap<(CreateCustomizeDto dto, Product product), CustomizeProduct>()
                 .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.product.ProductId))
                 .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.dto.UserId))
                 .ForMember(dest => dest.ShirtColor, opt => opt.MapFrom(src => src.dto.ShirtColor))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dto.Description))
                 .ForMember(dest => dest.DesignMetadata, opt => opt.MapFrom(src => src.dto.DesignMetadata))
                 .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.dto.Price));



        }
    }
}
