using System.Diagnostics;
using System.Net;

namespace ChatApi.Application.Responses
{
    public interface IErrorResponse
    {
        public string Type { get; }
        public string Title { get; }
        public int Status { get; }
        public string TraceId { get; }
    }

    public class UnauthorizedResponse : IErrorResponse
    {
        public UnauthorizedResponse(string traceparent)
        {
            TraceId = traceparent;
        }

        public string Type => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";

        public string Title => "Access Denied";

        public int Status => 401;

        public string TraceId { get; set; }
    }

    public class NotFoundResponse : IErrorResponse
    {
        public NotFoundResponse(string traceparent, string message)
        {
            TraceId = traceparent;
            Errors = new { message };
        }

        public string Type => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";

        public string Title => "Not Found";

        public int Status => 404;

        public string TraceId { get; set; }

        public object Errors { get; set; }
    }

    public class BadRequestResponse : IErrorResponse
    {
        public BadRequestResponse(string traceparent, string message)
        {
            TraceId = traceparent;
            Errors = new { message };
        }

        public string Type => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";

        public string Title => "Bad Request";

        public int Status => 400;

        public string TraceId { get; set; }

        public object Errors { get; set; }
    }

    public class ErrorResponseFactory
    {
        public ErrorResponseFactory() {}

        public IErrorResponse CreateErrorResponse(
            HttpStatusCode statusCode,
            string traceparent = null,
            string message = null)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new UnauthorizedResponse(traceparent);
                case HttpStatusCode.NotFound:
                    return new NotFoundResponse(traceparent, message);
                case HttpStatusCode.BadRequest:
                    return new NotFoundResponse(traceparent, message);
                default:
                    return new UnauthorizedResponse(traceparent);
            }
        }
    }
}
