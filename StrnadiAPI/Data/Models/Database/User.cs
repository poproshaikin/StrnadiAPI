namespace StrnadiAPI.Data.Models.Database;

/// <summary>
/// tabulka uživatelů - možná ještě budou přibývat sloupečky a měnit se datové typy
/// </summary>
public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }
    
    public string? Nickname { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public bool IsEmailVerified { get; set; }

    /// <summary>
    /// přírodovědci dodají seznam &quot;souhlasů s&quot; - abychom byli schopni přiřadit hodnoty
    /// </summary>
    public bool Consent { get; set; }

    public virtual ICollection<Recording> Recordings { get; set; } = new List<Recording>();
}
