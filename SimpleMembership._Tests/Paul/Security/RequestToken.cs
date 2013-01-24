using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snaptech.DataAccessManager.Attributes;
using System.Data;

namespace PPS.API.Common.Security
{
	public class RequestToken : TokenInfo
	{
        [Data("Callback", SqlDbType.VarChar, 256)]
        public string Callback { get; set; }

        [Data("Verifier", SqlDbType.VarChar, 32)]
        public string Verifier { get; set; }

        [Data("AuthorizerId", SqlDbType.UniqueIdentifier, 16)]
        public Guid? AuthorizerId { get; set; }
	}
}
