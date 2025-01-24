using System;
using System.Collections.Generic;

namespace StrnadiAPI.Data.Models.Database;

public partial class UserObject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }
}
