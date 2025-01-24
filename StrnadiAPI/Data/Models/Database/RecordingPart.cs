using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class RecordingPart
{
    public int Id { get; set; }

    public int RecordingId { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public decimal GpsLatitudeStart { get; set; }

    public decimal GpsLongitudeStart { get; set; }

    public decimal GpsLatitudeEnd { get; set; }

    public decimal GpsLongitude { get; set; }

    public string? Square { get; set; }

    public string FilePath { get; set; } = null!;

    public virtual ICollection<FiltredSubrecording> FiltredSubrecordings { get; set; } = new List<FiltredSubrecording>();

    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

    public virtual Recording Recording { get; set; } = null!;

    public virtual ICollection<UserPart> UserParts { get; set; } = new List<UserPart>();
}
