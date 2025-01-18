namespace StrnadiAPI.Data.Models.Database;

/// <summary>
/// Vysekané signifikantní části z nahrávek (např. 5ti vteřinové úseky se zaznamenaným dialektem). Výstup prvního AI modelu pro další zpracování a určení dialektu druhým AI modelem.
/// </summary>
public partial class FiltredSubrecording
{
    public int Id { get; set; }

    public int RecordingPartsId { get; set; }

    public int? BirdsId { get; set; }

    public string PathFile { get; set; } = null!;

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    /// <summary>
    /// pravděpodobnostní vektor jednotlivých dialektů (s jakou mírou pravděpodobnosti se v nahrávce vyskytují: dialekt A, dialekt B, dialekt C,...)
    /// </summary>
    public string ProbabilityVector { get; set; } = null!;

    /// <summary>
    /// 0-vloženo\n1-vytvořen pravděpodobnostní vektor\n2-pravděpodobnost dialektu X přesáhla nastavenou hranici a AI sama přiřadila dialekt\n3- přiřazení dialektu ověřeno uživatelem\n4- manuální přiřazení dialektu\n5- dialekt nelze určit ani manuálně
    /// </summary>
    public short State { get; set; }

    /// <summary>
    /// 1 - vzorek je reprezentant nahrávky\n0 - nereprezentuje nahrávku (podružný vzorek)
    /// </summary>
    public bool RepresentantFlag { get; set; }

    public virtual Bird? Birds { get; set; }

    public virtual RecordingPart RecordingParts { get; set; } = null!;
}
