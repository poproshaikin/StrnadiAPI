using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class DetectedDialect
{
    public int Id { get; set; }

    public int FiltredSubrecordingId { get; set; }

    public int DialectsId { get; set; }

    public virtual Dialect Dialects { get; set; } = null!;

    public virtual FiltredSubrecording FiltredSubrecording { get; set; } = null!;
}
