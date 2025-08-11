using AutoMapper;
using Entity.Base;
using Entity.Models;
using Business.DTOs.Property;

namespace Business.Mappings;

public class PropertyProfile : Profile
{
    public PropertyProfile()
    {
        // Property to DTOs
        CreateMap<Property, PropertyListDto>()
            .ForMember(dest => dest.CustomerName, 
                      opt => opt.MapFrom(src => src.Customer!.FullName))
            .ForMember(dest => dest.ProvinceName,
                      opt => opt.MapFrom(src => src.Province!.Name))
            .ForMember(dest => dest.DistrictName,
                      opt => opt.MapFrom(src => src.District!.Name))
            .ForMember(dest => dest.NeighborhoodName,
                      opt => opt.MapFrom(src => src.Neighborhood!.Name))
            .ForMember(dest => dest.TypeSpecificInfo,
                      opt => opt.MapFrom(src => GetTypeSpecificDisplayInfo(src)))
            .ForMember(dest => dest.TypeSpecificProperties,
                      opt => opt.MapFrom(src => GetTypeSpecificProperties(src)));

        CreateMap<Property, PropertyDetailDto>()
            .ForMember(dest => dest.CustomerName,
                      opt => opt.MapFrom(src => src.Customer!.FullName))
            .ForMember(dest => dest.CustomerPhone,
                      opt => opt.MapFrom(src => src.Customer!.Phone))
            .ForMember(dest => dest.ProvinceName,
                      opt => opt.MapFrom(src => src.Province!.Name))
            .ForMember(dest => dest.DistrictName,
                      opt => opt.MapFrom(src => src.District!.Name))
            .ForMember(dest => dest.NeighborhoodName,
                      opt => opt.MapFrom(src => src.Neighborhood!.Name))
            .ForMember(dest => dest.TypeSpecificData,
                      opt => opt.MapFrom(src => GetTypeSpecificData(src)));

        // Related entities
        CreateMap<PropertyPriceHistory, PriceHistoryDto>();
        CreateMap<PropertyDocument, PropertyDocumentDto>();
    }

    private string GetTypeSpecificDisplayInfo(Property property)
    {
        return property.PropertyType switch
        {
            Entity.Enums.PropertyType.Apartment => "Daire",
            Entity.Enums.PropertyType.Field => "Tarla",
            Entity.Enums.PropertyType.Land => "Arsa", 
            Entity.Enums.PropertyType.Commercial => "İşyeri",
            Entity.Enums.PropertyType.SharedParcel => "Hisseli",
            _ => "Bilinmiyor"
        };
    }

    private string GetTypeSpecificDetailInfo(Property property)
    {
        return property.PropertyType switch
        {
            Entity.Enums.PropertyType.Apartment => "Daire",
            Entity.Enums.PropertyType.Field => "Tarla",
            Entity.Enums.PropertyType.Land => "Arsa", 
            Entity.Enums.PropertyType.Commercial => "İşyeri",
            Entity.Enums.PropertyType.SharedParcel => "Hisseli",
            _ => "Bilinmiyor"
        };
    }

    private PropertyTypeSpecificDto GetTypeSpecificData(Property property)
    {
        var properties = new Dictionary<string, object>();

        switch (property)
        {
            case FieldProperty field:
                properties["BlockNumber"] = field.BlockNumber;
                properties["ParcelNumber"] = field.ParcelNumber;
                properties["TotalArea"] = field.TotalArea;
                properties["PricePerSquareMeter"] = field.PricePerSquareMeter;
                properties["TotalPrice"] = field.TotalPrice;
                properties["RoadStatus"] = field.RoadStatus;
                properties["FieldType"] = field.FieldType.ToString();
                // HasShareholder için default değer - front-end tarafından kontrol edilecek
                properties["HasShareholder"] = false; // Default değer, front-end güncelleyecek
                break;
                
            case LandProperty land:
                properties["BlockNumber"] = land.BlockNumber;
                properties["ParcelNumber"] = land.ParcelNumber;
                properties["TotalArea"] = land.TotalArea;
                properties["PricePerSquareMeter"] = land.PricePerSquareMeter;
                properties["TotalPrice"] = land.TotalPrice;
                properties["ZoningStatus"] = land.ZoningStatus.ToString();
                properties["LandType"] = land.LandType.ToString();
                break;
                
            case WorkplaceProperty workplace:
                properties["WorkplaceType"] = workplace.WorkplaceType.ToString();
                properties["TotalAreaGross"] = workplace.TotalAreaGross;
                properties["TotalAreaNet"] = workplace.TotalAreaNet;
                properties["RoomCount"] = workplace.RoomCount;
                properties["BathroomCount"] = workplace.BathroomCount;
                properties["TotalPrice"] = workplace.TotalPrice;
                properties["HeatingType"] = workplace.HeatingType.ToString();
                properties["MezzanineStatus"] = workplace.MezzanineStatus.ToString();
                properties["BasementStatus"] = workplace.BasementStatus.ToString();
                properties["UsageStatus"] = workplace.UsageStatus.ToString();
                break;
                
            case ApartmentProperty apartment:
                properties["Floor"] = apartment.Floor;
                properties["RoomCount"] = apartment.RoomCount;
                properties["BathroomCount"] = apartment.BathroomCount;
                properties["BalconyCount"] = apartment.BalconyCount;
                properties["LivingRoomCount"] = apartment.LivingRoomCount;
                properties["ParkingCount"] = apartment.ParkingCount;
                properties["Elevator"] = apartment.Elevator;
                properties["HeatingType"] = apartment.HeatingType.ToString();
                properties["ElevatorType"] = apartment.ElevatorType.ToString();
                properties["ParkingType"] = apartment.ParkingType.ToString();
                properties["FornitureStatus"] = apartment.FornitureStatus.ToString();
                properties["TotalAreaGross"] = apartment.TotalAreaGross;
                properties["TotalAreaNet"] = apartment.TotalAreaNet;
                properties["TotalPrice"] = apartment.TotalPrice;
                break;
                
            case SharedParcelProperty sharedParcel:
                properties["BlockNumber"] = sharedParcel.BlockNumber;
                properties["ParcelNumber"] = sharedParcel.ParcelNumber;
                properties["TotalArea"] = sharedParcel.TotalArea;
                properties["PricePerSquareMeter"] = sharedParcel.PricePerSquareMeter;
                properties["TotalPrice"] = sharedParcel.TotalPrice;
                properties["ShareRatio"] = sharedParcel.ShareRatio;
                break;
        }

        return new PropertyTypeSpecificDto
        {
            PropertyType = property.PropertyType,
            Properties = properties
        };
    }

    private Dictionary<string, object> GetTypeSpecificProperties(Property property)
    {
        var properties = new Dictionary<string, object>();

        switch (property)
        {
            case FieldProperty field:
                properties["BlockNumber"] = field.BlockNumber;
                properties["ParcelNumber"] = field.ParcelNumber;
                properties["TotalArea"] = field.TotalArea;
                properties["PricePerSquareMeter"] = field.PricePerSquareMeter;
                properties["TotalPrice"] = field.TotalPrice;
                properties["RoadStatus"] = field.RoadStatus;
                properties["FieldType"] = field.FieldType.ToString();
                // HasShareholder için default değer - front-end tarafından kontrol edilecek
                properties["HasShareholder"] = false; // Default değer, front-end güncelleyecek
                break;
                
            case LandProperty land:
                properties["BlockNumber"] = land.BlockNumber;
                properties["ParcelNumber"] = land.ParcelNumber;
                properties["TotalArea"] = land.TotalArea;
                properties["PricePerSquareMeter"] = land.PricePerSquareMeter;
                properties["TotalPrice"] = land.TotalPrice;
                properties["ZoningStatus"] = land.ZoningStatus.ToString();
                properties["LandType"] = land.LandType.ToString();
                break;
                
            case WorkplaceProperty workplace:
                properties["WorkplaceType"] = workplace.WorkplaceType.ToString();
                properties["TotalAreaGross"] = workplace.TotalAreaGross;
                properties["TotalAreaNet"] = workplace.TotalAreaNet;
                properties["RoomCount"] = workplace.RoomCount;
                properties["BathroomCount"] = workplace.BathroomCount;
                properties["TotalPrice"] = workplace.TotalPrice;
                properties["HeatingType"] = workplace.HeatingType.ToString();
                properties["MezzanineStatus"] = workplace.MezzanineStatus.ToString();
                properties["BasementStatus"] = workplace.BasementStatus.ToString();
                properties["UsageStatus"] = workplace.UsageStatus.ToString();
                break;
                
            case ApartmentProperty apartment:
                properties["Floor"] = apartment.Floor;
                properties["RoomCount"] = apartment.RoomCount;
                properties["BathroomCount"] = apartment.BathroomCount;
                properties["BalconyCount"] = apartment.BalconyCount;
                properties["LivingRoomCount"] = apartment.LivingRoomCount;
                properties["ParkingCount"] = apartment.ParkingCount;
                properties["Elevator"] = apartment.Elevator;
                properties["HeatingType"] = apartment.HeatingType.ToString();
                properties["ElevatorType"] = apartment.ElevatorType.ToString();
                properties["ParkingType"] = apartment.ParkingType.ToString();
                properties["FornitureStatus"] = apartment.FornitureStatus.ToString();
                properties["TotalAreaGross"] = apartment.TotalAreaGross;
                properties["TotalAreaNet"] = apartment.TotalAreaNet;
                properties["TotalPrice"] = apartment.TotalPrice;
                break;
                
            case SharedParcelProperty sharedParcel:
                properties["BlockNumber"] = sharedParcel.BlockNumber;
                properties["ParcelNumber"] = sharedParcel.ParcelNumber;
                properties["TotalArea"] = sharedParcel.TotalArea;
                properties["PricePerSquareMeter"] = sharedParcel.PricePerSquareMeter;
                properties["TotalPrice"] = sharedParcel.TotalPrice;
                properties["ShareRatio"] = sharedParcel.ShareRatio;
                break;
        }

        return properties;
    }
} 