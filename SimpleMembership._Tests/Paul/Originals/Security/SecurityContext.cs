using System.Collections.Generic;
using System.Security.Principal;

namespace PPS.API.Common.Security
{
	public class SecurityContext : ISecurityContext, IPrincipal
	{
		public SecurityContext() : this(null, null) { }
		public SecurityContext(ConsumerInfo consumerInfo) : this(consumerInfo, null) { }
		public SecurityContext(ConsumerInfo consumerInfo, PPS.API.Common.Security.Model.TokenBase tokenInfo)
		{
			this.ConsumerInfo = consumerInfo;
			this.TokenInfo = tokenInfo;

			this.Parameters = new Dictionary<string, string>();
		}

		public string Timestamp { get; set; }
		public string Signature { get; set; }
		public Dictionary<string, string> Parameters { get; set; }

		public AuthorizationProtocol Protocol { get; set; }
		public ConsumerInfo ConsumerInfo { get; set; }

        // Add "$type" property containing type info of concrete class.
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects)]
		public PPS.API.Common.Security.Model.TokenBase TokenInfo { get; set; }
		public List<PPS.API.Common.Security.Model.AccessGrant> Grants { get; set; }

		#region IPrincipal implementation

		public IIdentity Identity
		{
			get { return this.ConsumerInfo as IIdentity; }
		}

		public bool IsInRole(string role)
		{
			return true;
		}

		#endregion
	}
}