using System;
using System.Net.Http;
using SS.OAuth1.Client.Messages;
using SS.OAuth1.Client.Models;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client.Commands
{
    public class GetTokenCommand
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(GetTokenCommand));
        private MessageSender _messageSender;
        private MessageFactory _messageFactory;
        
        public MessageFactory MessageFactory
        {
            get { return _messageFactory ?? (_messageFactory = new MessageFactory()); }
        }

        public MessageSender MessageSender
        {
            get { return _messageSender = _messageSender ?? new MessageSender(); }
            set { _messageSender = value; }
        }

        public Creds GetToken(OAuthParametersBase parameters)
        {
            var msg = this.MessageFactory.Create(parameters);
            var authHeader = parameters.GetOAuthHeader();
            msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);
            var response = this.MessageSender.Send(msg);
            
            var token = ExtractToken(response);

            return token;
        }

        protected virtual Creds ExtractToken(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsFormDataAsync().Result;

            if (result == null) throw new Exception("No content found.");

            var key = result[Keys.TOKEN];
            var secret = result[Keys.TOKEN_SECRET];
            var token = new Creds(key, secret);

            return token;
        }
    }
}
