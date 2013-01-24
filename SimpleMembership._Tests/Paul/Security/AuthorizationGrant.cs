using Snaptech.DataAccessManager.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PPS.API.Common.Security
{
    public class AuthorizationGrant
    {
        [Data("MerchantId", SqlDbType.Int, 4)]
        public int MerchantId;

        [Data("RoleId", SqlDbType.Int, 4)]
        public int RoleId;

        [Data("RoleType", SqlDbType.VarChar, 50)]
        public RoleType Role;
    }
}
