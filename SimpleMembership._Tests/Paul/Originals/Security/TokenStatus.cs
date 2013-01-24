using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPS.API.Common.Security
{
	public enum TokenStatus
	{
		Inactive = 0,
		Active = 1,
		Verified = 2,
		Expired = 3,
		Revoked = 4,
	}
}
