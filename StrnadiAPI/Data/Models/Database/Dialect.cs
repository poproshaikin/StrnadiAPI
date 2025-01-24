using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class Dialect
{
    public int Id { get; set; }

    public string DialectCode { get; set; } = null!;

    public string PathSpectrogram { get; set; } = null!;

    public string PathVoice { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<DetectedDialect> DetectedDialects { get; set; } = new List<DetectedDialect>();
}
