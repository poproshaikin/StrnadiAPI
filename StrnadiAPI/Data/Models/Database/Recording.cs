namespace StrnadiAPI.Data.Models.Database;

/// <summary>
/// Tabulka s originálními - dlouhými nahrávkami.
/// </summary>
public partial class Recording
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? RecordingFilePath { get; set; }

    /// <summary>
    /// 1, 2, 3-více
    /// </summary>
    public short EstimatedBirdsCount { get; set; }

    /// <summary>
    /// 0-založeno\n1-další zpracování - je třeba rozmyslet stavy, jakými zpracování může probíhat a jakých výsledků může být zpracováním dosaženo\n...
    /// </summary>
    // 
    public short State { get; set; }

    /// <summary>
    /// čím bylo nahrané (předá přímo mobilní telefon, případně uživatel zadá u externích zařízení)\n\nbylo by fajn nabízet uživateli poslední zařízení, co vkládal (+ našeptávač)\n\n???u uživatele přidat defaultní zařízení???\n\n- přidat políčko přes_ appku nebo jinak
    /// </summary>
    public string Device { get; set; }

    /// <summary>
    /// 1 - nahráno s palikace, 0 - nahráno jinak
    /// </summary>
    public bool ByApp { get; set; }

    public string? Note { get; set; }

    public string? NotePost { get; set; }

    public virtual ICollection<RecordingPart>? RecordingParts { get; set; }

    public virtual User? User { get; set; }
}
