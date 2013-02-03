//using System;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
//using SS.OAuth.Models;
//using SS.OAuth.Models.Parameters;
//using log4net;

//namespace SS.OAuth.Commands
//{
//    public class GetRequestTokenCommand
//    {
//        private static readonly ILog LOG = LogManager.GetLogger(typeof(GetRequestTokenCommand));
        
        
//        private MessageSender _messageSender;

//        public MessageSender MessageSender
//        {
//            get { return _messageSender = _messageSender ?? new MessageSender(); }
//            set { _messageSender = value; }
//        }

        
//        public Creds GetToken(BaseParams paramz)
//        {
//            var msg = this.CreateMessage(paramz);
//            AddOAuthHeader(paramz, msg);
//            var response = this.MessageSender.Send(msg);

//            var token = ExtractToken(response);

//            return token;
//        }

//        public HttpRequestMessage CreateMessage(IMessageParameters paramz)
//        {
//            var msg = new HttpRequestMessage(paramz.HttpMethod, paramz.RequestUri);
//            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
//            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

//            return msg;
//        }

//        private static void AddOAuthHeader(OAuthParametersBase parameters, HttpRequestMessage msg)
//        {
//            var authHeader = parameters.GetOAuthHeader();
//            if (!string.IsNullOrEmpty(authHeader))
//            {
//                msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, "OAuth " + authHeader);
//            }
//        }

//        protected virtual Creds ExtractToken(HttpResponseMessage response)
//        {
//            var result = response.Content.ReadAsFormDataAsync().Result;

//            if (result == null) throw new Exception("No content found.");

//            var key = result[OAuth.V1.Keys.TOKEN];
//            var secret = result[OAuth.V1.Keys.TOKEN_SECRET];
//            var token = new Creds(key, secret);

//            return token;
//        }
//    }
//}