using System;

namespace Entity.Enums;

public enum PropertyType
{
    Land,
    Field,
    Apartment,
    Commercial,
    SharedParcel
}

public enum CustomerType
{
    Alıcı,
    Satıcı,
    AlıcıSatıcı,
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Cancelled
}

public enum PropertyStatus
{
    Satilik,
    Kiralik,
    SatilikKiralik,
    Rezerv,
    Satildi,
    Kiralandi
}

public enum LandZoneStatus
{
    Var,
    Yok,
    Belirsiz
}

public enum LandType
{
    Arsa,
    Sanayi,
    Çiftlik,
    Belirsiz
}

public enum FieldType
{
    Tarla,
    Bağ,
    Bahçe,
    Belirsiz
}

public enum HeatingType
{
    Merkezi,
    MerkeziPayölçer,
    Kalorifer,
    Kombi,
    Elektrikli,
    Soba,
    Klima,
    YerdenIsıtma,
    Yok,
    Belirsiz
}
public enum ElevatorType
{
    Var,
    Yok,
    Belirsiz
}

public enum ParkingType
{
    VarAçık,
    VarKapalı,
    Yok,
    Belirsiz

}
public enum FornitureStatus
{
    Eşyalı,
    Eşyasız,
    KısmenEşyalı,
    Belirsiz
}
public enum WorkplaceType
{
    Satılık,
    Kiralık,
    DevrenKiralık,
    DevrenSatılık,
    Belirsiz
}

public enum MezzanineStatus
{
    Var,
    Yok,
    Belirsiz
}

public enum BasementStatus
{
    Var,
    Yok,
    Belirsiz
}

public enum UsageStatus
{
    Boş,
    Dolu,
    DoluKiracılı,
    Belirsiz
}

// Müşteri ilgi alanları için genel enum
public enum InterestType
{
    // Arsa kategorisi
    Arsa,
    SanayiArsası,
    ÇiftlikArsası,
    ArsadanHisse,
    
    // Tarla kategorisi  
    Tarla,
    Bağ,
    Bahçe,
    TarladanHisse,
    
    // Daire kategorisi
    Daire,
    KiralıkDaire,
    
    // İşyeri kategorisi
    İşyeri,
    Kiralıkİşyeri,
    
    // Genel
    Tümü
}