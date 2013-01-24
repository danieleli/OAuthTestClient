
using System.Data;
using System.Security.Principal;
using Snaptech.DataAccessManager.Attributes;

namespace PPS.API.Common.Security
{
	public class ConsumerInfo : IIdentity
	{
		[Data("ConsumerId", SqlDbType.UniqueIdentifier, 8)]
		public System.Guid Id { get; set; }

        [Data("ConsumerType", SqlDbType.VarChar, 256)]
        public ConsumerType ConsumerType { get; set; }
		
		[Data("ConsumerName", SqlDbType.VarChar, 256)]
		public string Name { get; set; }

		[Data("ConsumerEmail", SqlDbType.VarChar, 256)]
		public string Email { get; set; }

		[Data("ConsumerKey", SqlDbType.VarChar, 128)]
		public string Key { get; set; }

		[Data("ConsumerSecret", SqlDbType.VarChar, 1024)]
		public string Secret { get; set; }

        public string AuthenticationType
        {
            get { return this.ConsumerType.ToString(); }
        }

		public bool IsAuthenticated
		{
			get { return true; }
		}
	}
}
