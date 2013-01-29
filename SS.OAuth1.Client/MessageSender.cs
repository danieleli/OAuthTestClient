#region

using System.Net.Http;
using log4net;

#endregion

namespace SS.OAuth1.Client
{
    public interface IMessageSender
    {
        HttpResponseMessage Send(HttpRequestMessage msg);
    }

    public class MessageSender : IMessageSender
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MessageSender));
        private HttpClient _httpClient;
        private ResponseValidator _validator;

        public ResponseValidator Validator
        {
            get { return _validator ?? (_validator = new ResponseValidator()); }
            set { _validator = value; }
        }

        public HttpClient HttpClient
        {
            get { return _httpClient ?? (_httpClient = new HttpClient()); }
            set { _httpClient = value; }
        }

        public HttpResponseMessage Send(HttpRequestMessage msg)
        {
            LOG.Debug("Send Message: \n" + msg + "\n");

            var response = this.HttpClient.SendAsync(msg).Result;
            
            LOG.Debug("Message Response: \n" + response + "\n");
            this.Validator.ValidateResponse(response);

            return response;
        }
    }
}