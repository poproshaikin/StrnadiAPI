namespace StrnadiAPI.Controllers;

public enum UploadError
{
    StreamInterrupted = 1,
    DbConflict,
    DoesntExist
}