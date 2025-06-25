using AutoMapper;
using Entity.Models;
using Entity.Base;
using Business.DTOs.Customer;

namespace Business.Mappings;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        // Customer to DTOs
        CreateMap<Customer, CustomerListDto>()
            .ForMember(dest => dest.ProvincePreferencesCount, 
                      opt => opt.MapFrom(src => src.ProvincePreferences.Count));

        CreateMap<Customer, CustomerDetailDto>()
            .ForMember(dest => dest.ProvincePreferences, 
                      opt => opt.MapFrom(src => src.ProvincePreferences))
            .ForMember(dest => dest.Properties,
                      opt => opt.MapFrom(src => src.Properties));

        // DTOs to Customer
        CreateMap<CustomerCreateDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ProvincePreferences, opt => opt.Ignore())
            .ForMember(dest => dest.Properties, opt => opt.Ignore());

        CreateMap<CustomerUpdateDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ProvincePreferences, opt => opt.Ignore())
            .ForMember(dest => dest.Properties, opt => opt.Ignore());

        // Province preferences
        CreateMap<CustomerProvincePreference, ProvincePreferenceDetailDto>()
            .ForMember(dest => dest.ProvinceName, 
                      opt => opt.MapFrom(src => src.Province!.Name))
            .ForMember(dest => dest.DistrictPreferences,
                      opt => opt.MapFrom(src => src.DistrictPreferences));

        CreateMap<CustomerDistrictPreference, DistrictPreferenceDto>()
            .ForMember(dest => dest.DistrictName,
                      opt => opt.MapFrom(src => src.District!.Name));

        // Property summary
        CreateMap<Property, PropertySummaryDto>();
    }
} 