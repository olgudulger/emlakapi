using System;
using Entity.Base;

namespace Entity.Models;

public class PropertyPriceHistory : BaseEntity
{
    public int PropertyId { get; set; }
    public Property Property { get; set; }

    public decimal Price { get; set; }
    public DateTime Date { get; set; }
}
