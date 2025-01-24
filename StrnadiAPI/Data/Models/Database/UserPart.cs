using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class UserPart
{
    public int Id { get; set; }

    public int RecordingPartId { get; set; }

    public int SecondsFrom { get; set; }

    public int SecondsTo { get; set; }

    public string Note { get; set; } = null!;

    public virtual RecordingPart RecordingPart { get; set; } = null!;
}
