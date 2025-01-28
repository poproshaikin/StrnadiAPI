using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Data.Repositories;

public interface IRecordingsRepository
{
    AddResult Add(Recording recording);

    AddResult Add<TProperty>(Recording recording, 
        Expression<Func<Recording, TProperty>> returning,
        out TProperty? value);
    
    IReadOnlyList<Recording> Get();
    
    Recording? GetById(int id);
    
    UpdateResult Update(Recording updated);
    
    DeleteResult Delete(int id);
    
    bool Exists(int resultRecordingId);
}

public class RecordingsRepository : IRecordingsRepository
{
    private StrnadiDbContext _context;

    public RecordingsRepository(StrnadiDbContext context)
    {
        _context = context;
    }
    
    public AddResult Add(Recording recording)
    {
        try
        {
            _context.Recordings.Add(recording);
            _context.SaveChanges();
            return AddResult.Success;
        }
        catch (DbUpdateException)
        {
            return AddResult.Fail;
        }
    }

    public AddResult Add<TProperty>(Recording recording,
        Expression<Func<Recording, TProperty>> returning,
        out TProperty? value)
    {
        try
        {
            EntityEntry<Recording> addedEntity = _context.Recordings.Add(recording);
            _context.SaveChanges();
            value = returning.Compile()(addedEntity.Entity);
            return AddResult.Success;
        }
        catch (DbUpdateException)
        {
            value = default;
            return AddResult.Fail;
        }
    }

    public IReadOnlyList<Recording> Get()
    {
        return _context.Recordings.ToArray();
    }

    public Recording? GetById(int id)
    {
        return _context.Recordings.FirstOrDefault(r => r.Id == id);
    }

    public UpdateResult Update(Recording updated)
    {
        if (updated.Id == default)
        {
            return UpdateResult.IdNotProvided;
        }
        
        Recording? recording = _context.Recordings.FirstOrDefault(r => r.Id == updated.Id);
        
        if (recording is null)
        {
            return UpdateResult.NotFound;
        }
        
        _context.Recordings.Update(recording);

        try
        {
            _context.SaveChanges();
            return UpdateResult.Successful;
        }
        catch (DbUpdateException)
        {
            return UpdateResult.Fail;
        }
    }

    public DeleteResult Delete(int id)
    {
        Recording? recording = _context.Recordings.FirstOrDefault(r => r.Id == id);
        
        if (recording is null)
        {
            return DeleteResult.NotFound;
        }

        try
        {
            _context.Recordings.Remove(recording);
            _context.SaveChanges();

            return DeleteResult.Success;
        }
        catch (DbUpdateException)
        {
            return DeleteResult.Fail;
        }
    }

    public bool Exists(int id)
    {
        return _context.Recordings.Any(r => r.Id == id);
    }
}