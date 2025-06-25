using AutoMapper;
using Entity.Models;
using Business.DTOs.Sale;
using DataAccess.Abstract;
using Entity.Enums;

namespace Business.Mappings;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        // Sale to DTOs
        CreateMap<Sale, SaleListDto>()
            .ForMember(dest => dest.PropertyTitle, opt => opt.MapFrom(src => src.Property.Title))
            .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.Property.PropertyType.ToString()))
            .ForMember(dest => dest.SellerCustomerName, opt => opt.MapFrom(src => src.Property.Customer.FullName))
            .ForMember(dest => dest.BuyerCustomerName, opt => opt.MapFrom(src => src.BuyerCustomer.FullName))
            .ForMember(dest => dest.NetProfit, opt => opt.MapFrom(src => src.NetProfit))
            .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => GetSaleStatusText(src.Status)))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

        CreateMap<Sale, SaleDetailDto>()
            .ForMember(dest => dest.PropertyTitle, opt => opt.MapFrom(src => src.Property.Title))
            .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.Property.PropertyType.ToString()))
            .ForMember(dest => dest.PropertyLocation, opt => opt.MapFrom(src => 
                $"{src.Property.Province.Name}, {src.Property.District.Name}, {src.Property.Neighborhood.Name}"))
            .ForMember(dest => dest.SellerCustomerId, opt => opt.MapFrom(src => src.Property.CustomerId))
            .ForMember(dest => dest.SellerCustomerName, opt => opt.MapFrom(src => src.Property.Customer.FullName))
            .ForMember(dest => dest.SellerCustomerPhone, opt => opt.MapFrom(src => src.Property.Customer.Phone))
            .ForMember(dest => dest.BuyerCustomerName, opt => opt.MapFrom(src => src.BuyerCustomer.FullName))
            .ForMember(dest => dest.BuyerCustomerPhone, opt => opt.MapFrom(src => src.BuyerCustomer.Phone))
            .ForMember(dest => dest.NetProfit, opt => opt.MapFrom(src => src.NetProfit))
            .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => GetSaleStatusText(src.Status)))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

        // DTOs to Sale
        CreateMap<SaleCreateDto, Sale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore())
            .ForMember(dest => dest.BuyerCustomer, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<SaleUpdateDto, Sale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore())
            .ForMember(dest => dest.BuyerCustomer, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Statistics mapping
        CreateMap<SaleStatistics, SaleStatisticsDto>();
    }

    private static string GetSaleStatusText(SaleStatus status)
    {
        return status switch
        {
            SaleStatus.Completed => "Tamamlandı",
            SaleStatus.Pending => "Beklemede",
            SaleStatus.Cancelled => "İptal Edildi",
            SaleStatus.Postponed => "Ertelendi",
            _ => status.ToString()
        };
    }
} 