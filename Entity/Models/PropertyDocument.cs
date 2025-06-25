using System;
using Entity.Base;

namespace Entity.Models;

public class PropertyDocument : BaseEntity
{
    public int PropertyId { get; set; }
    public required Property Property { get; set; }

    public required string FileName { get; set; }
    public required string FilePath { get; set; }
}
