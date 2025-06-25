using System;
using Entity.Base;

namespace Entity.Models;

public class SharedParcelProperty : Property
{
    public float ShareRatio { get; set; } = 1;
    public string BlockNumber { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public decimal TotalArea { get; set; }
    public decimal PricePerSquareMeter { get; set; }
    public decimal TotalPrice { get; set; } = 0;
}
