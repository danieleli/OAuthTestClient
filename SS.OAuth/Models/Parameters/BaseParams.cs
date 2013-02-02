using System.Collections.Specialized;
using SS.OAuth.Extensions;
using SS.OAuth.Misc;

namespace SS.OAuth.Models.Parameters
{

    public class BaseParams
    {
        #region -- Properties --

        private string _nonce;
        private string _timestamp;
        
        public const string SIGNATURE_METHOD = OAuth.V1.Values.SIGNATURE_METHOD;
        public const string VERSION = OAuth.V1.Values.VERSION;

        public Creds Consumer { get; protected set; }
        public Creds RequestToken { get; protected set; }
        public string Realm { get; protected set; }

        public virtual string Timestamp
        {
            get { return _timestamp ?? (_timestamp = Utils.GenerateTimeStamp()); }
        }

        public virtual string Nonce
        {
            get { return _nonce ?? (_nonce = Utils.GenerateNonce()); }
        }

        #endregion -- Properties --

        /// <summary>
        /// key - is set to the concatenated values of:
        /// 
        /// 1.  The client shared-secret, after being encoded (Section 3.6).
        ///
        /// 2.  An "&" character (ASCII code 38), which MUST be included
        /// even when either secret is empty.
        /// 
        /// 3.  The token shared-secret, after being encoded (Section 3.6). 
        /// </summary>
        public virtual string GetSignatureKey()
        {
            return this.Consumer.Secret.UrlEncodeForOAuth() + "&";
        }
    }
}
