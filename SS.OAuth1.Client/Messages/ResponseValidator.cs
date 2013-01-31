using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;

namespace SS.OAuth1.Client.Messages
{
    public class ResponseValidator
    {
        public void ValidateResponse(HttpResponseMessage response)
        {

            var isAuthorized = (response.StatusCode != HttpStatusCode.Unauthorized);
            if (!isAuthorized)
            {
                var ex = GetUnauthorizedException(response);
                throw ex;
            }

            var isServerError = (response.StatusCode == HttpStatusCode.InternalServerError);
            if (isServerError)
            {
                var ex = GetServerException(response);
                throw ex;
            }

            response.EnsureSuccessStatusCode();
        }

        private ServerException GetServerException(HttpResponseMessage response)
        {
            var msg = "Internal Server Error: '{0}' \nMessage: {1}\nDetails:{2}";
            var content = response.Content.ReadAsStringAsync().Result;
            content = Uri.UnescapeDataString(content);
            msg = string.Format(msg, response.ReasonPhrase, content, "det");
            var ex = new ServerException(msg);
            return ex;
        }

        private UnauthorizedAccessException GetUnauthorizedException(HttpResponseMessage response)
        {
            var values = response.Content.ReadAsFormDataAsync().Result;
            var msg = "";

            try
            {
                var errorCode = values["errorCode"];
                var message = values["message"];
                msg = string.Format("ErrorCode: {0}; Message: {1}", errorCode, message);
            }
            catch (Exception e)
            {
                msg = "No Message Found";
            }

            return new UnauthorizedAccessException(msg);

        }
    }
}