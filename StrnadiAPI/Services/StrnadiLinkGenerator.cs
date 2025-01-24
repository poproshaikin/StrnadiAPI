using System.Runtime.CompilerServices;
using StrnadiAPI.Controllers;

namespace StrnadiAPI.Services;

public class StrnadiLinkGenerator
{
    public StrnadiLinkGenerator()
    {
        
    }

    public string GenerateLink(HttpContext context, int userId)
    {
        string scheme = context.Request.Scheme;
        string host = context.Request.Host.ToUriComponent();

        string link = $"{scheme}://{host}/users/{nameof(UsersController.Register)}?guid={userId}";

        return link;
    }
}