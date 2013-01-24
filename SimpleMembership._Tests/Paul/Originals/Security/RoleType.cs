
using System;
namespace PPS.API.Common.Security
{
    [Serializable]
    public enum RoleType
    {
        Administrator = 0,
        Supervisor = 2,
        Clerk = 3,
        ReadOnly = 4,
        NoAccess = 5,
    }
}
