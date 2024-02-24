using Application.Common.Exceptions;
using System.Net;

namespace Application.Common.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
            StatusCode = 200; //success (OK)
            Succeeded = true;
        }

        public Response(CrudResponse crudResponse)
        {
            StatusCode = 200; //success (OK)
            Succeeded = true;
            Message = crudResponse.ToString();
        }

        public Response(T data, string message = null)
        {
            StatusCode = 200; //success (OK)
            Succeeded = true;
            Message = message;
            Data = data;
        }

        public Response(T data, CrudResponse response)
        {
            StatusCode = 200; //success (OK)
            Succeeded = true;
            Message = response.ToString();
            Data = data;
        }

        //for response error constructor
        public Response(HttpStatusCode statusCode, string message)
        {
            throw new ResponseException(statusCode, message);
        }

        public int StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public object Errors { get; }

        public T Data { get; set; }
    }
}
