using Resilentr.src.Common;
using System.Net;

namespace Resilentr.src.Resilentr.Client
{
    /// <summary>
    /// Customize exception for handling exception within the endpoint.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="customErrorMessage"></param>
    /// <param name="statusCode"></param>
    [AttributeUsage(AttributeTargets.Method)]
    public class ResilentrAttribute(AttributeProperty attribute, string? customErrorMessage = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : Attribute
    {
        private AttributeProperty _attribute = attribute;
        private string? _customErrorMessage = customErrorMessage;
        private HttpStatusCode? _statusCode = statusCode;
    }
}