using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snaptech.DataAccessManager.Attributes;
using System.Data;

namespace PPS.API.Common.Security
{
	public class TokenAccess
	{
		[Data("MerchantId", SqlDbType.Int, 4)]
		public int? MerchantId { get; set; }

		[Data("RoleId", SqlDbType.Int, 4)]
		public int? RoleId { get; set; }
	}
}
