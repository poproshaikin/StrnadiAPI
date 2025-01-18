namespace StrnadiAPI.Data.Models.Database;

/// <summary>
/// K nahrávkám může být jedna a více fotek. (Asi bych doporučoval ukládat i meta-data. Nicmoc nás to nebude stát a třeba se to bude někdy hodit - např při dalším zpracování pomocí AI.)\n\npřidat GPS k fotce
/// </summary>
public partial class Photo
{
    public int Id { get; set; }

    public int RecordingPartsId { get; set; }

    public string PhotoFilePath { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public string Device { get; set; } = null!;

    public decimal? GpsLatitude { get; set; }

    public decimal? GpsLongitude { get; set; }

    public virtual RecordingPart RecordingParts { get; set; } = null!;
}
