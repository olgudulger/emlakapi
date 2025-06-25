namespace Entity.Enums;

public enum SaleStatus
{
    /// <summary>
    /// Satış tamamlandı
    /// </summary>
    Completed = 1,
    
    /// <summary>
    /// Satış beklemede/işlem halinde
    /// </summary>
    Pending = 2,
    
    /// <summary>
    /// Satış iptal edildi
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Satış ertelendi
    /// </summary>
    Postponed = 4
} 