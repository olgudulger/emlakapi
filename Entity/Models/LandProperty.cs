using System;
using Entity.Base;
using Entity.Enums;

namespace Entity.Models;

public class LandProperty : Property
{
    // LandBaseProperty properties
    public string BlockNumber { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public decimal TotalArea { get; set; }
    public decimal PricePerSquareMeter { get; set; }
    public decimal TotalPrice { get; set; } = 0;
    
    // LandProperty specific properties
    public LandZoneStatus ZoningStatus { get; set; } = LandZoneStatus.Belirsiz;
    public LandType LandType { get; set; } = LandType.Arsa;
}
