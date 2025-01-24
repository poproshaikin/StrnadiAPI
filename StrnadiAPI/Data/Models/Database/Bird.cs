using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class Bird
{
    public int Id { get; set; }

    public string? Uid { get; set; }

    public string? Mfcc { get; set; }

    public virtual ICollection<FiltredSubrecording> FiltredSubrecordings { get; set; } = new List<FiltredSubrecording>();
}
