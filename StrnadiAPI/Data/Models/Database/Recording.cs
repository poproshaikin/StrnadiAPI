using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class Recording
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public short EstimatedBirdsCount { get; set; }

    public short State { get; set; }

    public string Device { get; set; } = null!;

    public bool ByApp { get; set; }

    public string? Note { get; set; }

    public string? NotePost { get; set; }

    public virtual ICollection<RecordingPart> RecordingParts { get; set; } = new List<RecordingPart>();

    public virtual User User { get; set; } = null!;
}
