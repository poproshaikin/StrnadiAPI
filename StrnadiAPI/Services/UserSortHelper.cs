using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Services;

public class UserSortHelper
{
    private const string default_sorting_column_name = "surname";
    private const string default_sorting_order = "ascending";

    // TODO
    
    public User[] SortBy(User[] users)
    {
        return users;
    }
    
    public User[] SortBy(User[] users, Dictionary<string, string> orderingRule)
    {
        return users;
    }
}