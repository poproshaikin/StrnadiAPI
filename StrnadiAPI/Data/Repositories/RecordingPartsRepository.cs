using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Data.Repositories;

public interface IRecordingPartsRepository
{
    AddResult Add(RecordingPart recordingPart);
    
    AddResult Add<TProperty>(RecordingPart recordingPart,
        Expression<Func<RecordingPart, TProperty>> returning,
        out TProperty? value);
    
    IReadOnlyList<RecordingPart> Get();
    
    RecordingPart? GetById(int id);
    
    UpdateResult Update(RecordingPart updated);
    
    DeleteResult Delete(int id);
    
    bool Exists(int recordingPartId);
}

public class RecordingPartsRepository : IRecordingPartsRepository
{
    private StrnadiDbContext _context;

    public RecordingPartsRepository(StrnadiDbContext context)
    {
        _context = context;
    }
    
    public AddResult Add(RecordingPart recordingPart)
    {
        try
        {
            _context.RecordingParts.Add(recordingPart);
            _context.SaveChanges();
            return AddResult.Success;
        }
        catch (DbUpdateException)
        {
            return AddResult.Fail;
        }
    }
    
    public AddResult Add<TProperty>(RecordingPart recordingPart,
        Expression<Func<RecordingPart, TProperty>> returning,
        out TProperty? value)
    {
        try
        {
            EntityEntry<RecordingPart> addedEntity = _context.RecordingParts.Add(recordingPart);
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

    public IReadOnlyList<RecordingPart> Get()
    {
        return _context.RecordingParts.ToList();
    }

    public RecordingPart? GetById(int id)
    {
        return _context.RecordingParts.FirstOrDefault(p => p.Id == id);
    }

    public UpdateResult Update(RecordingPart updated)
    {
        if (updated.Id == default)
        {
            return UpdateResult.IdNotProvided;
        }
        
        RecordingPart? recordingPart = _context.RecordingParts.FirstOrDefault(p => p.Id == updated.Id);
        
        if (recordingPart is null)
        {
            return UpdateResult.NotFound;
        }

        _context.RecordingParts.Update(recordingPart);

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
        RecordingPart? recordingPart = _context.RecordingParts.FirstOrDefault(p => p.Id == id);

        if (recordingPart is null)
        {
            return DeleteResult.NotFound;
        }

        try
        {
            _context.RecordingParts.Remove(recordingPart);
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
        return _context.RecordingParts.Any(p => p.Id == id);
    }
}