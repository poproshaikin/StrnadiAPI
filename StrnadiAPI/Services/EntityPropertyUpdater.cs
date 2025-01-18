using System.Reflection;
using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Services;

public interface IEntityPropertyUpdater
{
    bool Update(string propertyName, string propertyValue);
}

public class UserPropertyUpdater : IEntityPropertyUpdater
{
    private readonly User _user;

    public UserPropertyUpdater(User user)
    {
        _user = user;
    }

    public bool Update(string propertyName, string propertyValue)
    {
        
    }
}

// public class EntityPropertyUpdater<TEntity>
// {
//     private TEntity _entity;
//
//     public EntityPropertyUpdater(TEntity entity)
//     {
//         _entity = entity;
//     }
//
//     public bool Update(string propertyName, object propertyValue)
//     {
//         PropertyInfo? info = _entity.GetType().GetProperty(propertyName);
//
//         if (info is null)
//             return false;
//
//         try
//         {
//             info.SetValue(_entity, propertyValue, null);
//             return true;
//         }
//         catch
//         {
//             return false;
//         }
//     }
// }