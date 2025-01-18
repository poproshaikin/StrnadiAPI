namespace StrnadiAPI.Data.Models.Database;

public class UserObject
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public string Value { get; set; }
    
    public DateTime Start { get; set; }
    
    public DateTime End { get; set; }
}