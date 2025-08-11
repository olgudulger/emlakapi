using AutoMapper;
using Business.Abstract;
using Business.DTOs.Property;
using DataAccess.Abstract;
using Entity.Models;
using Entity.Base;
using Entity.Enums;
using System.Text.Json;

namespace Business.Concrete;

public class PropertyManager : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PropertyManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PropertyListDto>> GetAllAsync()
    {
        var properties = await _unitOfWork.Properties.GetAllAsync();
        return _mapper.Map<IEnumerable<PropertyListDto>>(properties);
    }

    public async Task<PropertyDetailDto?> GetByIdAsync(int id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null) return null;

        var propertyDetailDto = _mapper.Map<PropertyDetailDto>(property);
        
        // Fiyat geçmişini yükle
        var priceHistory = await GetPriceHistoryAsync(id);
        propertyDetailDto.PriceHistory = priceHistory.ToList();
        
        return propertyDetailDto;
    }

    public async Task<PropertyDetailDto> CreateAsync(PropertyCreateDto propertyCreateDto)
    {
        var property = CreatePropertyFromDto(propertyCreateDto);
        property.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Properties.AddAsync(property);
        await _unitOfWork.SaveChangesAsync();

        // Emlak oluşturulduğunda fiyat geçmişine ilk kaydı ekle
        await AddInitialPriceHistory(property);

        return await GetByIdAsync(property.Id) ?? throw new InvalidOperationException("Emlak oluşturulamadı");
    }

    /// <summary>
    /// Emlak oluşturulduğunda ilk fiyat geçmişi kaydını ekler
    /// </summary>
    private async Task AddInitialPriceHistory(Property property)
    {
        var totalPrice = GetTotalPriceFromProperty(property);
        if (totalPrice > 0)
        {
            var priceHistory = new PropertyPriceHistory
            {
                PropertyId = property.Id,
                Price = totalPrice,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PropertyPriceHistories.AddAsync(priceHistory);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Emlak türüne göre toplam fiyatı alır
    /// </summary>
    private decimal GetTotalPriceFromProperty(Property property)
    {
        return property switch
        {
            ApartmentProperty apartment => apartment.TotalPrice,
            FieldProperty field => field.TotalPrice,
            LandProperty land => land.TotalPrice,
            WorkplaceProperty workplace => workplace.TotalPrice,
            SharedParcelProperty sharedParcel => sharedParcel.TotalPrice,
            _ => 0
        };
    }

    private Property CreatePropertyFromDto(PropertyCreateDto dto)
    {
        Property property = dto.PropertyType switch
        {
            PropertyType.Apartment => new ApartmentProperty(),
            PropertyType.Field => new FieldProperty(),
            PropertyType.Land => new LandProperty(),
            PropertyType.Commercial => new WorkplaceProperty(),
            PropertyType.SharedParcel => new SharedParcelProperty(),
            _ => throw new InvalidOperationException($"Desteklenmeyen emlak türü: {dto.PropertyType}")
        };

        // Base property alanlarını mapla
        property.PropertyType = dto.PropertyType;
        property.Title = dto.Title;
        property.ProvinceId = dto.ProvinceId;
        property.DistrictId = dto.DistrictId;
        property.NeighborhoodId = dto.NeighborhoodId;
        property.IntermediaryFullName = dto.IntermediaryFullName;
        property.IntermediaryPhone = dto.IntermediaryPhone;
        property.Status = dto.Status;
        property.Notes = dto.Notes;
        property.CustomerId = dto.CustomerId;

        // Type-specific properties'i mapla
        MapTypeSpecificProperties(property, dto.TypeSpecificProperties);

        return property;
    }

    private void MapTypeSpecificProperties(Property property, Dictionary<string, object> typeSpecificProperties)
    {
        if (typeSpecificProperties == null || !typeSpecificProperties.Any())
            return;

        switch (property)
        {
            case ApartmentProperty apartment:
                MapApartmentProperties(apartment, typeSpecificProperties);
                break;
            case FieldProperty field:
                MapFieldProperties(field, typeSpecificProperties);
                break;
            case LandProperty land:
                MapLandProperties(land, typeSpecificProperties);
                break;
            case WorkplaceProperty workplace:
                MapWorkplaceProperties(workplace, typeSpecificProperties);
                break;
            case SharedParcelProperty sharedParcel:
                MapSharedParcelProperties(sharedParcel, typeSpecificProperties);
                break;
        }
    }

    private void MapApartmentProperties(ApartmentProperty apartment, Dictionary<string, object> properties)
    {
        if (properties.TryGetValue("Floor", out var floor))
            apartment.Floor = GetStringValue(floor);
        if (properties.TryGetValue("RoomCount", out var roomCount))
            apartment.RoomCount = GetIntValue(roomCount);
        if (properties.TryGetValue("BathroomCount", out var bathroomCount))
            apartment.BathroomCount = GetIntValue(bathroomCount);
        if (properties.TryGetValue("BalconyCount", out var balconyCount))
            apartment.BalconyCount = GetIntValue(balconyCount);
        if (properties.TryGetValue("LivingRoomCount", out var livingRoomCount))
            apartment.LivingRoomCount = GetIntValue(livingRoomCount);
        if (properties.TryGetValue("ParkingCount", out var parkingCount))
            apartment.ParkingCount = GetStringValue(parkingCount);
        if (properties.TryGetValue("Elevator", out var elevator))
            apartment.Elevator = GetStringValue(elevator);
        if (properties.TryGetValue("HeatingType", out var heatingType))
            apartment.HeatingType = (HeatingType)GetIntValue(heatingType);
        if (properties.TryGetValue("ElevatorType", out var elevatorType))
            apartment.ElevatorType = (ElevatorType)GetIntValue(elevatorType);
        if (properties.TryGetValue("ParkingType", out var parkingType))
            apartment.ParkingType = (ParkingType)GetIntValue(parkingType);
        if (properties.TryGetValue("FornitureStatus", out var fornitureStatus))
            apartment.FornitureStatus = (FornitureStatus)GetIntValue(fornitureStatus);
        if (properties.TryGetValue("TotalAreaGross", out var totalAreaGross))
            apartment.TotalAreaGross = GetIntValue(totalAreaGross);
        if (properties.TryGetValue("TotalAreaNet", out var totalAreaNet))
            apartment.TotalAreaNet = GetIntValue(totalAreaNet);
        if (properties.TryGetValue("TotalPrice", out var totalPrice))
            apartment.TotalPrice = GetDecimalValue(totalPrice);
    }

    private void MapFieldProperties(FieldProperty field, Dictionary<string, object> properties)
    {
        if (properties.TryGetValue("BlockNumber", out var blockNumber))
            field.BlockNumber = GetStringValue(blockNumber);
        if (properties.TryGetValue("ParcelNumber", out var parcelNumber))
            field.ParcelNumber = GetStringValue(parcelNumber);
        if (properties.TryGetValue("TotalArea", out var totalArea))
            field.TotalArea = GetDecimalValue(totalArea);
        if (properties.TryGetValue("PricePerSquareMeter", out var pricePerSquareMeter))
            field.PricePerSquareMeter = GetDecimalValue(pricePerSquareMeter);
        if (properties.TryGetValue("TotalPrice", out var totalPrice))
            field.TotalPrice = GetDecimalValue(totalPrice);
        if (properties.TryGetValue("RoadStatus", out var roadStatus))
            field.RoadStatus = GetStringValue(roadStatus);
        if (properties.TryGetValue("FieldType", out var fieldType))
            field.FieldType = (FieldType)GetIntValue(fieldType);
        if (properties.TryGetValue("HasShareholder", out var hasShareholder))
            field.HasShareholder = GetBoolValue(hasShareholder);
    }

    private void MapLandProperties(LandProperty land, Dictionary<string, object> properties)
    {
        if (properties.TryGetValue("BlockNumber", out var blockNumber))
            land.BlockNumber = GetStringValue(blockNumber);
        if (properties.TryGetValue("ParcelNumber", out var parcelNumber))
            land.ParcelNumber = GetStringValue(parcelNumber);
        if (properties.TryGetValue("TotalArea", out var totalArea))
            land.TotalArea = GetDecimalValue(totalArea);
        if (properties.TryGetValue("PricePerSquareMeter", out var pricePerSquareMeter))
            land.PricePerSquareMeter = GetDecimalValue(pricePerSquareMeter);
        if (properties.TryGetValue("TotalPrice", out var totalPrice))
            land.TotalPrice = GetDecimalValue(totalPrice);
        if (properties.TryGetValue("ZoningStatus", out var zoningStatus))
            land.ZoningStatus = (LandZoneStatus)GetIntValue(zoningStatus);
        if (properties.TryGetValue("LandType", out var landType))
            land.LandType = (LandType)GetIntValue(landType);
    }

    private void MapWorkplaceProperties(WorkplaceProperty workplace, Dictionary<string, object> properties)
    {
        if (properties.TryGetValue("WorkplaceType", out var workplaceType))
            workplace.WorkplaceType = (WorkplaceType)GetIntValue(workplaceType);
        if (properties.TryGetValue("TotalAreaGross", out var totalAreaGross))
            workplace.TotalAreaGross = GetIntValue(totalAreaGross);
        if (properties.TryGetValue("TotalAreaNet", out var totalAreaNet))
            workplace.TotalAreaNet = GetIntValue(totalAreaNet);
        if (properties.TryGetValue("RoomCount", out var roomCount))
            workplace.RoomCount = GetIntValue(roomCount);
        if (properties.TryGetValue("BathroomCount", out var bathroomCount))
            workplace.BathroomCount = GetIntValue(bathroomCount);
        if (properties.TryGetValue("TotalPrice", out var totalPrice))
            workplace.TotalPrice = GetDecimalValue(totalPrice);
        if (properties.TryGetValue("HeatingType", out var heatingType))
            workplace.HeatingType = (HeatingType)GetIntValue(heatingType);
        if (properties.TryGetValue("MezzanineStatus", out var mezzanineStatus))
            workplace.MezzanineStatus = (MezzanineStatus)GetIntValue(mezzanineStatus);
        if (properties.TryGetValue("BasementStatus", out var basementStatus))
            workplace.BasementStatus = (BasementStatus)GetIntValue(basementStatus);
        if (properties.TryGetValue("UsageStatus", out var usageStatus))
            workplace.UsageStatus = (UsageStatus)GetIntValue(usageStatus);
    }

    private void MapSharedParcelProperties(SharedParcelProperty sharedParcel, Dictionary<string, object> properties)
    {
        if (properties.TryGetValue("BlockNumber", out var blockNumber))
            sharedParcel.BlockNumber = GetStringValue(blockNumber);
        if (properties.TryGetValue("ParcelNumber", out var parcelNumber))
            sharedParcel.ParcelNumber = GetStringValue(parcelNumber);
        if (properties.TryGetValue("TotalArea", out var totalArea))
            sharedParcel.TotalArea = GetDecimalValue(totalArea);
        if (properties.TryGetValue("PricePerSquareMeter", out var pricePerSquareMeter))
            sharedParcel.PricePerSquareMeter = GetDecimalValue(pricePerSquareMeter);
        if (properties.TryGetValue("TotalPrice", out var totalPrice))
            sharedParcel.TotalPrice = GetDecimalValue(totalPrice);
        if (properties.TryGetValue("ShareRatio", out var shareRatio))
            sharedParcel.ShareRatio = GetFloatValue(shareRatio);
    }

    // Helper methods for safe type conversion
    private string GetStringValue(object value)
    {
        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.String ? jsonElement.GetString() ?? "" : jsonElement.ToString();
        }
        return value?.ToString() ?? "";
    }

    private int GetIntValue(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Number)
            {
                return jsonElement.GetInt32();
            }
            if (jsonElement.ValueKind == JsonValueKind.String && int.TryParse(jsonElement.GetString(), out int result))
            {
                return result;
            }
        }
        
        if (value is int intValue) return intValue;
        if (value is string strValue && int.TryParse(strValue, out int parsedValue)) return parsedValue;
        
        return Convert.ToInt32(value);
    }

    private decimal GetDecimalValue(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Number)
            {
                return jsonElement.GetDecimal();
            }
            if (jsonElement.ValueKind == JsonValueKind.String && decimal.TryParse(jsonElement.GetString(), out decimal result))
            {
                return result;
            }
        }
        
        if (value is decimal decValue) return decValue;
        if (value is double doubleValue) return (decimal)doubleValue;
        if (value is float floatValue) return (decimal)floatValue;
        if (value is int intValue) return intValue;
        if (value is string strValue && decimal.TryParse(strValue, out decimal parsedValue)) return parsedValue;
        
        return Convert.ToDecimal(value);
    }

    private float GetFloatValue(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Number)
            {
                return jsonElement.GetSingle();
            }
            if (jsonElement.ValueKind == JsonValueKind.String && float.TryParse(jsonElement.GetString(), out float result))
            {
                return result;
            }
        }
        
        if (value is float floatValue) return floatValue;
        if (value is double doubleValue) return (float)doubleValue;
        if (value is decimal decValue) return (float)decValue;
        if (value is string strValue && float.TryParse(strValue, out float parsedValue)) return parsedValue;
        
        return Convert.ToSingle(value);
    }

    private bool GetBoolValue(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.True) return true;
            if (jsonElement.ValueKind == JsonValueKind.False) return false;
            if (jsonElement.ValueKind == JsonValueKind.String && bool.TryParse(jsonElement.GetString(), out bool result))
            {
                return result;
            }
        }
        
        if (value is bool boolValue) return boolValue;
        if (value is string strValue && bool.TryParse(strValue, out bool parsedValue)) return parsedValue;
        
        return Convert.ToBoolean(value);
    }

    public async Task<PropertyDetailDto?> UpdateAsync(int id, PropertyUpdateDto propertyUpdateDto)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null) return null;

        // Güncellemeden önceki fiyatı al
        var oldPrice = GetTotalPriceFromProperty(property);

        // Base property alanlarını güncelle
        property.Title = propertyUpdateDto.Title;
        property.ProvinceId = propertyUpdateDto.ProvinceId;
        property.DistrictId = propertyUpdateDto.DistrictId;
        property.NeighborhoodId = propertyUpdateDto.NeighborhoodId;
        property.IntermediaryFullName = propertyUpdateDto.IntermediaryFullName;
        property.IntermediaryPhone = propertyUpdateDto.IntermediaryPhone;
        property.Status = propertyUpdateDto.Status;
        property.Notes = propertyUpdateDto.Notes;
        property.UpdatedAt = DateTime.UtcNow;

        // TypeSpecificProperties'i güncelle
        if (propertyUpdateDto.TypeSpecificProperties != null && propertyUpdateDto.TypeSpecificProperties.Any())
        {
            MapTypeSpecificProperties(property, propertyUpdateDto.TypeSpecificProperties);
        }

        await _unitOfWork.SaveChangesAsync();

        // Fiyat değiştiyse fiyat geçmişine ekle
        var newPrice = GetTotalPriceFromProperty(property);
        if (newPrice != oldPrice && newPrice > 0)
        {
            await AddPriceHistoryAsync(property.Id, newPrice);
        }

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null) return false;

        await _unitOfWork.Properties.DeleteAsync(property);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PropertyListDto>> GetByCustomerIdAsync(int customerId)
    {
        // Database-side filtreleme kullan
        var properties = await _unitOfWork.Properties.GetPropertiesByCustomerAsync(customerId);
        return _mapper.Map<IEnumerable<PropertyListDto>>(properties);
    }

    public async Task<IEnumerable<PropertyListDto>> GetByPropertyTypeAsync(PropertyType propertyType)
    {
        // Database-side filtreleme kullan
        var properties = await _unitOfWork.Properties.GetPropertiesByTypeAsync(propertyType);
        return _mapper.Map<IEnumerable<PropertyListDto>>(properties);
    }

    public async Task<IEnumerable<PropertyListDto>> GetByLocationAsync(int provinceId, int? districtId = null)
    {
        // Database-side filtreleme kullan
        var properties = await _unitOfWork.Properties.GetPropertiesByLocationAsync(provinceId, districtId);
        return _mapper.Map<IEnumerable<PropertyListDto>>(properties);
    }

    public async Task<IEnumerable<PropertyListDto>> SearchAsync(string searchTerm)
    {
        // Database-side arama kullan - N+1 problemini çöz
        var properties = await _unitOfWork.Properties.SearchPropertiesAsync(searchTerm);
        return _mapper.Map<IEnumerable<PropertyListDto>>(properties);
    }

    public async Task<IEnumerable<PropertyListDto>> GetByStatusAsync(PropertyStatus status)
    {
        // Database-side filtreleme kullan
        var properties = await _unitOfWork.Properties.GetPropertiesByStatusAsync(status);
        return _mapper.Map<IEnumerable<PropertyListDto>>(properties);
    }

    // Yeni performans optimize edilmiş method'lar
    public async Task<IEnumerable<PropertyListDto>> GetPropertiesPaginatedAsync(int page, int pageSize, PropertyStatus? status = null)
    {
        var skip = (page - 1) * pageSize;
        var properties = await _unitOfWork.Properties.GetPropertiesPaginatedAsync(skip, pageSize, status);
        return _mapper.Map<IEnumerable<PropertyListDto>>(properties);
    }

    public async Task<int> GetPropertiesCountAsync(PropertyStatus? status = null)
    {
        return await _unitOfWork.Properties.GetPropertiesCountAsync(status);
    }

    public async Task<IEnumerable<PropertyListDto>> GetPropertiesByMultipleStatusAsync(PropertyStatus[] statuses)
    {
        var properties = await _unitOfWork.Properties.GetPropertiesByMultipleStatusAsync(statuses);
        return _mapper.Map<IEnumerable<PropertyListDto>>(properties);
    }

    public async Task<bool> AddPriceHistoryAsync(int propertyId, decimal price)
    {
        try
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null) return false;

            // Önceki fiyatla aynıysa ekleme
            var latestPrice = await _unitOfWork.PropertyPriceHistories.GetLatestPriceByPropertyAsync(propertyId);
            if (latestPrice != null && latestPrice.Price == price) return false;

            var priceHistory = new PropertyPriceHistory
            {
                PropertyId = propertyId,
                Price = price,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PropertyPriceHistories.AddAsync(priceHistory);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<PriceHistoryDto>> GetPriceHistoryAsync(int propertyId)
    {
        var priceHistory = await _unitOfWork.PropertyPriceHistories.GetPriceHistoryByPropertyAsync(propertyId);
        return _mapper.Map<IEnumerable<PriceHistoryDto>>(priceHistory);
    }

    public async Task<bool> AddDocumentAsync(int propertyId, PropertyDocumentCreateDto documentDto)
    {
        // Will be implemented later
        return true;
    }

    public async Task<bool> RemoveDocumentAsync(int documentId)
    {
        // Will be implemented later  
        return true;
    }

    public async Task<IEnumerable<PropertyDocumentDto>> GetDocumentsAsync(int propertyId)
    {
        // Will be implemented later
        return new List<PropertyDocumentDto>();
    }
} 