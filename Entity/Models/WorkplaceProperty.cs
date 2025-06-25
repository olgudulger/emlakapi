using System;
using Entity.Base;
using Entity.Enums;

namespace Entity.Models;

public class WorkplaceProperty : Property
{
    public WorkplaceType WorkplaceType { get; set; } = WorkplaceType.Belirsiz;
    public MezzanineStatus MezzanineStatus { get; set; } = MezzanineStatus.Belirsiz;
    public BasementStatus BasementStatus { get; set; } = BasementStatus.Belirsiz;
    public UsageStatus UsageStatus { get; set; } = UsageStatus.Belirsiz;
    public int TotalAreaGross { get; set; } = 0;
    public int TotalAreaNet { get; set; } = 0;
    public int BathroomCount { get; set; } = 0;
    public int RoomCount { get; set; } = 0;
    public decimal TotalPrice { get; set; } = 0;
    public HeatingType HeatingType { get; set; } = HeatingType.Belirsiz;
}