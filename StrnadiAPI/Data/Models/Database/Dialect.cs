namespace StrnadiAPI.Data.Models.Database;

/// <summary>
/// Číselník dialektů
/// </summary>
public partial class Dialect
{
    public int Id { get; set; }

    public string DialectCode { get; set; } = null!;

    public string PathSpectrogram { get; set; } = null!;

    /// <summary>
    /// cesta k souboru se vzorovým zvukovým záznamem dialektu
    /// </summary>
    public string PathVoice { get; set; } = null!;

    public string Description { get; set; } = null!;
}
