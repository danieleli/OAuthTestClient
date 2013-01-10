using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.OAuth2;


namespace SimpleMembership.Auth.OAuth2
{
    public class MxClient2 : WebServerClient
    {
        
        
            private static readonly AuthorizationServerDescription FacebookDescription = new AuthorizationServerDescription
            {
                TokenEndpoint = new Uri("https://graph.facebook.com/oauth/access_token"),
                AuthorizationEndpoint = new Uri("https://graph.facebook.com/oauth/authorize"),
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="FacebookClient"/> class.
            /// </summary>
            public MxClient2()
                : base(FacebookDescription)
            {
            }
        }
    
}