using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class User
{
    public int Id { get; set; }

    public string? Nickname { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public bool? IsEmailVerified { get; set; }

    public bool? Consent { get; set; }

    public virtual ICollection<Recording> Recordings { get; set; } = new List<Recording>();
}
