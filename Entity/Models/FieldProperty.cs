using System;
using Entity.Base;
using Entity.Enums;

namespace Entity.Models;

public class FieldProperty : Property
{
    // LandBaseProperty properties
    public string BlockNumber { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public decimal TotalArea { get; set; }
    public decimal PricePerSquareMeter { get; set; }
    public decimal TotalPrice { get; set; } = 0;
    
    // FieldProperty specific properties
    public string RoadStatus { get; set; } = string.Empty;
    public FieldType FieldType { get; set; } = FieldType.Belirsiz;
    public bool HasShareholder { get; set; } = false;

}
