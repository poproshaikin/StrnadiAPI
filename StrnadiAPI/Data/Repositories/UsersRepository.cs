using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;
using StrnadiAPI.Services;

namespace StrnadiAPI.Data.Repositories;

public interface IUsersRepository
{
    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="user">
    /// The user object to be added. The object must contain valid data for all required properties.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the user was successfully added.
    /// Returns <c>true</c> if the addition was successful, <c>false</c> otherwise (e.g., if a user with the same unique columns already exists).
    /// </returns>
    bool Add(User user);
    
    /// <summary>
    /// Retrieves a list of users and orders them based on the provided JSON string.
    /// If no sorting rule is specified (null), the default sorting is by the "Surname" column in ascending order.
    /// </summary>
    /// <param name="orderingJson">
    /// A JSON string where keys are column names and values are sorting directions ("ascending" or "descending").
    /// Example: { "Name": "ascending", "Age": "descending" }.
    /// If null, the default ordering is applied: ascending by the "Surname" column.
    /// </param>
    /// <returns>
    /// A read-only list of users ordered according to the specified or default sorting rule.
    /// </returns>
    IReadOnlyList<User> GetAndOrder(string? orderingJson);
    
    User? Get(int id);
    
    /// <summary>
    /// Updates the properties of an object based on the provided JSON string.
    /// The JSON must contain property names as keys and their new values as values. 
    /// The object to be updated is identified by the "id" property, which is required.
    /// </summary>
    /// <param name="changesJson">
    /// A JSON string where keys are property names and values are the new values to be assigned. 
    /// Example: { "id": 1, "Name": "John", "Age": 30 }.
    /// The "id" property is mandatory and used to identify the object to update.
    /// </param>
    /// <returns>
    /// An <see cref="UpdateResult"/> that indicates the outcome of the update operation
    /// </returns>
    /// <remarks>
    /// Currently is not supported, throws a <see cref="NotImplementedException"/>
    /// </remarks>
    UpdateResult Update(string changesJson);

    UpdateResult Update(User userNew);
    
    /// <summary>
    /// Deletes a user from the collection or database by their unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the user to be deleted.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the user was successfully deleted.
    /// Returns <c>true</c> if the user with the specified ID was found and deleted, <c>false</c> otherwise.
    /// </returns>
    bool Delete(int id);
}

public class UsersRepository : IUsersRepository
{
    private StrnadiDbContext _context;

    private DbSet<User> _users => _context.Users;
    
    public UsersRepository(StrnadiDbContext context)
    {
        _context = context;
    }
    
    public bool Add(User user)
    {
        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }
        catch (DbUpdateException e)
        {
            return false;
        }
    }

    public IReadOnlyList<User> GetAndOrder(string? orderingJson)
    {
        JsonHelper json = new(orderingJson ?? "{ \"surname\": \"ascending\" }");
        UserSortHelper sorter = new();

        var users = _users.ToArray();
        var sorted = sorter.SortBy(users, json.Properties);

        return sorted;
    }

    public User? Get(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public UpdateResult Update(User userNew)
    {
        int id = userNew.Id;
        User? userOld = _users.FirstOrDefault(u => u.Id == id);

        if (userOld is null)
        {
            return UpdateResult.NotFound;
        }

        if (!CheckEmailUnique(userNew))
        {
            return UpdateResult.UniquenessViolation;
        }
        
        _context.Users.Update(userOld);

        try
        {
            _context.SaveChanges();
            return UpdateResult.Successful;
        }
        catch (DbUpdateException e)
        {
            return UpdateResult.Fail;
        }
    }
    
    public UpdateResult Update(string changesJson)
    {
        JsonHelper json = new(changesJson);

#pragma warning disable CS8600 // In that case idString is always not-null when "id" key exists
        if (!json.Properties.TryGetValue("id", out string idString))
#pragma warning restore CS8600 //
        {
            return UpdateResult.WrongIdProvided;
        }
        
        int id = int.Parse(idString);
        
        User? user = _users.FirstOrDefault(u => u.Id == id);

        if (user is null)
        {
            return UpdateResult.NotFound;
        }

        UserPropertyUpdater updater = new(user);

        // TODO
        throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
        User? user = _users.FirstOrDefault(u => u.Id == id);

        if (user is null)
            return false;
        
        _context.Users.Remove(user!);
        _context.SaveChanges();
        
        return true;
    }

    private bool CheckEmailUnique(User user)
    {
        return _users.Any(u => u.Email != user.Email);
    }
}