using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snaptech.DataAccessManager.Attributes;

namespace PPS.API.Common.Security
{
	public class AuthorizationCode
	{
		[Data("Code", System.Data.SqlDbType.UniqueIdentifier, 16)]
		private Guid? _code
		{
			get { return new Guid(Convert.FromBase64String(Code + "==")); }
			set { Code = value.HasValue ? Convert.ToBase64String(value.Value.ToByteArray()).Replace("=", "") : null; }
		}
		public string Code { get; set; }

		[Data("RedirectUri", System.Data.SqlDbType.VarChar, 2048)]
		public string RedirectUri { get; set; }

		[Data("State", System.Data.SqlDbType.VarChar, 2048)]
		public string State { get; set; }
	}
}
