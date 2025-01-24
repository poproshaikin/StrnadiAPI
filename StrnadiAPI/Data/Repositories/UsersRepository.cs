using Microsoft.EntityFrameworkCore;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Data.Repositories;

public interface IUsersRepository
{
    AddResult Add(User user);
    
    IReadOnlyList<User> Get();
    
    User? Get(int id);
    
    UpdateResult Update(User updated);
    
    DeleteResult Delete(int id);

    void ConfirmEmail(string userId);
    
    LoginResult TryLogin(string email, string password);
}

public class UsersRepository : IUsersRepository
{
    private StrnadiDbContext _context;

    private DbSet<User> _users => _context.Users;
    
    public UsersRepository(StrnadiDbContext context)
    {
        _context = context;
    }
    
    public AddResult Add(User user)
    {
        // TODO perform a query to validate the uniqueness of all unique columns
        
        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return AddResult.Success;
        }
        catch (DbUpdateException)
        {
            return AddResult.Fail;
        }
    }

    public IReadOnlyList<User> Get()
    {
        return _users.Select(user => new User()
        {
            Id = user.Id,
            Nickname = user.Nickname, 
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsEmailVerified = user.IsEmailVerified,
            Consent = user.Consent,
            CreationDate = user.CreationDate
        }).ToArray();
    }

    public User? Get(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public UpdateResult Update(User updated)
    {
        User? user = _users.FirstOrDefault(u => u.Id == updated.Id);

        if (user is null)
        {
            return UpdateResult.NotFound;
        }

        if (!CheckEmailUnique(updated))
        {
            return UpdateResult.UniquenessViolation;
        }
        
        _context.Users.Update(user);

        try
        {
            _context.SaveChanges();
            return UpdateResult.Success;
        }
        catch (DbUpdateException)
        {
            return UpdateResult.Fail;
        }
    }

    public DeleteResult Delete(int id)
    {
        User? user = _users.FirstOrDefault(u => u.Id == id);

        if (user is null)
            return DeleteResult.NotFound;

        try
        {
            _context.Users.Remove(user!);
            _context.SaveChanges();
            
            return DeleteResult.Success;
        }
        catch (DbUpdateException)
        {
            return DeleteResult.Fail;
        }
    }

    public LoginResult TryLogin(string email, string password)
    {
        User? user = _users.FirstOrDefault(u => u.Email == email);

        if (user is null)
        {
            return LoginResult.EmailNotFound;
        }

        if (user.Password == password)
        {
            return LoginResult.Success;
        }
        
        return LoginResult.WrongPassword;
    }

    public void ConfirmEmail(string userId)
    {
        int userIdInt = int.Parse(userId);
        User user = _users.FirstOrDefault(u => u.Id == userIdInt)!;
        
        user.IsEmailVerified = true;
        _context.SaveChanges();
    }

    private bool CheckEmailUnique(User user)
    {
        return _users.Any(u => u.Email != user.Email);
    }
}