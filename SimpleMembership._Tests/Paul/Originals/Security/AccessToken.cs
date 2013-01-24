using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snaptech.DataAccessManager.Attributes;
using System.Data;

namespace PPS.API.Common.Security
{
	public class AccessToken : TokenInfo
	{
        [Data("RequestTokenId", SqlDbType.UniqueIdentifier, 8)]
        public Guid RequestTokenId { get; set; }
	}
}
