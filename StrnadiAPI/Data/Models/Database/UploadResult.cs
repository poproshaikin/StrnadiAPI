using StrnadiAPI.Controllers;

namespace StrnadiAPI.Data.Models.Database;

public class UploadResult
{
    public int Id { get; set; }
    public int RecordingId { get; set; }
    public byte[]? Data { get; set; }
    public UploadError? Error { get; set; }

    public UploadResult(int id = 0, byte[]? data = null, UploadError? error = null)
    {
        Id = id;
        Data = data;
        Error = error;
    }
}