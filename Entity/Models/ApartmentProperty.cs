using System;
using Entity.Base;
using Entity.Enums;

namespace Entity.Models;

public class ApartmentProperty : Property
{
    public string Floor { get; set; } = string.Empty;
    public int RoomCount { get; set; } = 0;
    public int BathroomCount { get; set; } = 0;
    public int BalconyCount { get; set; } = 0;
    public int LivingRoomCount { get; set; } = 0;
    public string ParkingCount { get; set; } = string.Empty;
    public string Elevator { get; set; } = string.Empty;
    public HeatingType HeatingType { get; set; } = HeatingType.Belirsiz;
    public ElevatorType ElevatorType { get; set; } = ElevatorType.Belirsiz;
    public ParkingType ParkingType { get; set; } = ParkingType.Belirsiz;
    public FornitureStatus FornitureStatus { get; set; } = FornitureStatus.Belirsiz;
    public int TotalAreaGross { get; set; } = 0;
    public int TotalAreaNet { get; set; } = 0;
    public decimal TotalPrice { get; set; } = 0;
}
