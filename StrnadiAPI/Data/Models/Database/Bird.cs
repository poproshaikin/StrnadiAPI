using System;
using System.Collections.Generic;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Data.Models;

public partial class Bird
{
    public int Id { get; set; }

    /// <summary>
    /// poprvé zaznamenán kdy a kde
    /// </summary>
    public string Uid { get; set; } = null!;

    /// <summary>
    /// mel-frekvenční spektrální koeficient
    /// </summary>
    public string? Mfcc { get; set; }

    public virtual ICollection<FiltredSubrecording> FiltredSubrecordings { get; set; } = new List<FiltredSubrecording>();
}
