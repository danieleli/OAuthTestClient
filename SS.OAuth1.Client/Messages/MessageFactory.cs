using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SS.OAuth1.Client.Parameters;

namespace SS.OAuth1.Client.Messages
{
    public class MessageFactory
    {
        public HttpRequestMessage Create(IMessageParameters paramz)
        {
            var msg = new HttpRequestMessage(paramz.HttpMethod, paramz.RequestUri);
            AddMediaFormatter(msg);

            return msg;
        }

        private void AddMediaFormatter(HttpRequestMessage msg)
        {
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }
    }
}
