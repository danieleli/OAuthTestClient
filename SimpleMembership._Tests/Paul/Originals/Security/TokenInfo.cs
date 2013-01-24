
using System;
using System.Data;
using Snaptech.DataAccessManager.Attributes;
namespace PPS.API.Common.Security
{
	public class TokenInfo
	{
        [Data("Id", SqlDbType.UniqueIdentifier, 8)]
        public Guid Id { get; set; }

        [Data("ConsumerId", SqlDbType.UniqueIdentifier, 8)]
        public Guid ConsumerId { get; set; }

        [Data("ConsumerType", SqlDbType.VarChar, 256)]
        public ConsumerType ConsumerType { get; set; }

        [Data("Token", SqlDbType.VarChar, 32)]
		public string Token { get; set; }

		[Data("Secret", SqlDbType.VarChar, 1024)]
		public string Secret { get; set; }

        [Data("IsActive", SqlDbType.Bit, 1)]
        public bool? Active { get; set; }

        [Data("IsAuthorized", SqlDbType.Bit, 1)]
        public bool? Authorized { get; set; }

        [Data("UtcDateExpires", SqlDbType.DateTime, 8)]
        public DateTime? UtcDateExpires { get; set; }

		[Data("UtcDateCreated", SqlDbType.DateTime, 8)]
		public DateTime UtcDateCreated { get; set; }
	}
}
