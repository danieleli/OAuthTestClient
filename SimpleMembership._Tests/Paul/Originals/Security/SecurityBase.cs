using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPS.API.Common.Security
{
	public abstract class SecurityBase : INeedSecurityContext
	{
		public ConsumerInfo ConsumerInfo
		{
			get
			{
				return this.SecurityContext.ConsumerInfo;
			}
		}

		public Guid ConsumerId
		{
			get
			{
				return this.SecurityContext.ConsumerInfo.Id;
			}
		}

		public override ISecurityContext SecurityContext
		{
			set { _securityContext = value; }
			get
			{
				if (_securityContext == null)
					_securityContext = base.SecurityContext;

				return _securityContext;
			}
		}
		private ISecurityContext _securityContext = null;
	}
}
