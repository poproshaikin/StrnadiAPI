﻿using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class Photo
{
    public int Id { get; set; }

    public int RecordingPartId { get; set; }

    public string? PhotoFilePath { get; set; }

    public DateTime CreationDate { get; set; }

    public decimal? GpsLatitude { get; set; }

    public decimal? GpsLongitude { get; set; }

    public virtual RecordingPart RecordingPart { get; set; } = null!;
}
