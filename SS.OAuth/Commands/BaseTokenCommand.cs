﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SS.OAuth.Extensions;
using SS.OAuth.Factories;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Commands
{
    public class BaseTokenCommand
    {
        protected BaseTokenCommand(BaseParams p)
        {
            SigFactory = new SignatureFactory(p);
        }

        private SignatureFactory SigFactory { get; set; }

        protected string CreateHeader( string sig )
        {
            var header = this.SigFactory.CreateHeader(sig);
            return header;
        }

        protected string CreateHeader( HttpRequestMessage msg )
        {
            throw new NotImplementedException();
        }

        protected string GetSignature( HttpRequestMessage msg )
        {
            var sig = this.SigFactory.GetSignature(msg);
            return sig;
        }

        protected Creds ExtractToken( HttpResponseMessage response )
        {
            var result = response.Content.ReadAsFormDataAsync().Result;

            if (result == null) throw new Exception("No content found.");

            var key = result[OAuth.V1.Keys.TOKEN];
            var secret = result[OAuth.V1.Keys.TOKEN_SECRET];
            var token = new Creds(key, secret);

            return token;
        }
    }
}