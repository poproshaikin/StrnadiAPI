using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class FiltredSubrecording
{
    public int Id { get; set; }

    public int RecordingPartId { get; set; }

    public int? BirdsId { get; set; }

    public string PathFile { get; set; } = null!;

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string? ProbabilityVector { get; set; }

    public bool RepresentantFlag { get; set; }

    public virtual Bird? Birds { get; set; }

    public virtual ICollection<DetectedDialect> DetectedDialects { get; set; } = new List<DetectedDialect>();

    public virtual RecordingPart RecordingPart { get; set; } = null!;
}
