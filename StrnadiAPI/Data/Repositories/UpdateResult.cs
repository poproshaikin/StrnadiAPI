namespace StrnadiAPI.Data.Repositories;

/// <summary>
/// Represents the result of an update operation.
/// </summary>
public enum UpdateResult
{
    /// <summary>
    /// The update operation was successful.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The update operation failed due to a uniqueness constraint violation.
    /// </summary>
    UniquenessViolation,

    /// <summary>
    /// The update operation failed because the "id" was not provided in the JSON.
    /// </summary>
    IdNotProvided,
    
    WrongIdProvided,
    
    /// <summary>
    /// The update operation failed because the entity with provided identifier does not exist.
    /// </summary>
    NotFound,
    
    /// <summary>
    /// The update operation failed because of any reason that was not specified.
    /// </summary>
    Fail
}