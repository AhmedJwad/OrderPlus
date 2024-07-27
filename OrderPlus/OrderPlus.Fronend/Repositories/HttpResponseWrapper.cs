using System.Net;

namespace OrderPlus.Fronend.Repositories
{
    public class HttpResponseWrapper<T>
    {
        public HttpResponseWrapper(T? response, bool error, HttpResponseMessage httpResponseMessage)
        {
            response = Response;
            error = Error;
           httpResponseMessage = HttpResponseMessage;
               
               
        }
        public T? Response { get; }
        public bool Error { get; }
        public HttpResponseMessage HttpResponseMessage { get; }

        public async Task<string> GetErrorMessageAsync()
        {
            if (!Error)
            {
                return null;
            }
            var statusCode=HttpResponseMessage.StatusCode;
            if(statusCode==HttpStatusCode.NotFound)
            {
                return "Recourse not found.";
            }
            if (statusCode == HttpStatusCode.BadRequest)
            {
                return await HttpResponseMessage.Content.ReadAsStringAsync();
            }
            if (statusCode == HttpStatusCode.Unauthorized)
            {
                return "You must be logged in to perform this operation.";
            }
            if (statusCode == HttpStatusCode.Forbidden)
            {
                return "You do not have permission to perform this operation.";
            }
            return "An unexpected error has occurred.";
        }

    }


}
