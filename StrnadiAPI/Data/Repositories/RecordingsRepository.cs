using Microsoft.EntityFrameworkCore;
using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Data.Repositories;

public interface IRecordingsRepository
{
    bool Add(Recording recording);
    
    IReadOnlyList<Recording> GetAll();
    
    Recording GetById(int id);
    
    UpdateResult Update(Recording recording);
    
    bool Delete(Recording recording);
}

public class RecordingsRepository : IRecordingsRepository
{
    private StrnadiDbContext _context;

    public RecordingsRepository(StrnadiDbContext context)
    {
        _context = context;
    }
    
    public bool Add(Recording recording)
    {
        try
        {
            _context.Recordings.Add(recording);
            _context.SaveChanges();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public IReadOnlyList<Recording> GetAll()
    {
        return _context.Recordings.ToArray();
    }

    public Recording GetById(int id)
    {
        return _context.Recordings.FirstOrDefault(r => r.Id == id)!;
    }

    public UpdateResult Update(Recording recording)
    {
        int id = recording.Id;
    }
}