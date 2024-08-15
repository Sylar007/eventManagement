using Celebratix.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace Celebratix.Common.Helpers;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params Enums.Role[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
