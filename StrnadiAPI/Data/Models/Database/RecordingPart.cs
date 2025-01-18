namespace StrnadiAPI.Data.Models.Database;

/// <summary>
/// Originální nahrávka může být slepena z jednotlivých částí. Bylo by fajn ukládat i informace o těchto částech. Mohou se tam vyskytnout třeba nějaké zdroje chyb pro zpracování AI.
/// </summary>
public partial class RecordingPart
{
    public int Id { get; set; }

    public int RecordingsId { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public decimal GpsLatitudeStart { get; set; }

    public decimal GpsLongitudeStart { get; set; }

    public decimal GpsLatitudeEnd { get; set; }

    public decimal GpsLongitudeEnd { get; set; }

    /// <summary>
    /// určení KFME čtverce ve formátu 0000a (56°0\N 5°40\E)\n\nPokud by bylo GPS mimo rozsah KFME, byl by squre NULL
    /// </summary>
    public string? Square { get; set; }

    public string FilePath { get; set; } = null!;

    public virtual ICollection<FiltredSubrecording> FiltredSubrecordings { get; set; } = new List<FiltredSubrecording>();

    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

    public virtual Recording Recordings { get; set; } = null!;
}
