using System;
using Entity.Base;

namespace Entity.Models;

public class Province : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<District> Districts { get; set; } = new List<District>();
}
