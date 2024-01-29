using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Resilentr.src.Common;
using Resilentr.src.Resilentr.Client;
using System.Net;
using System.Reflection;

namespace Resilentr.src.Resilentr.Middleware
{
    public class ResilentrMiddleware(RequestDelegate next, ILogger<ResilentrMiddleware> logger) 
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ResilentrMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            var attribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<ResilentrAttribute>();
            if (attribute is not null) 
            {
                AttributeProperty attributeProperty = GetAttributeProperty(attribute) ?? AttributeProperty.Default;
                string? customErrorMessage = GetCustomErrorMessage(attribute);
                HttpStatusCode statusCode = GetHttpStatusCode(attribute) ?? HttpStatusCode.InternalServerError;

                switch (attributeProperty)
                {
                    case AttributeProperty.Silent:
                        try
                        {
                            await _next(context);
                        }
                        catch(Exception e) {
                            await context.Response.WriteAsync($"{customErrorMessage}");
                            _logger.LogError(
                                $"Endpoint:{context.Features.Get<IEndpointFeature>()?.Endpoint} Attribute:Silent. Custom error message: {customErrorMessage}. Custom Response status: {(int)statusCode} " +
                                $"Exception message: {e.Message} " +
                                $"Inner exception: {e.InnerException?.Message ?? "N/a"}");
                        }
                        break;

                    case AttributeProperty.Loud:
                        try
                        {
                            await _next(context);
                        }
                        catch (Exception e)
                        {
                            context.Response.StatusCode = (int)statusCode;
                            await context.Response.WriteAsync($"{customErrorMessage ?? e.Message}");
                            _logger.LogError(
                                $"Endpoint:{context.Features.Get<IEndpointFeature>()?.Endpoint} Attribute:Loud. Custom error message: {customErrorMessage}. Custom Response status: {(int)statusCode} " +
                                $"Exception message: {e.Message} " +
                                $"Inner exception: {e.InnerException?.Message ?? "N/a"}");
                        }
                        break;

                    case AttributeProperty.Default:
                        try
                        {
                            await _next(context);
                        }
                        catch (Exception e)
                        {
                            context.Response.StatusCode = (int)statusCode;
                            await context.Response.WriteAsync($"{customErrorMessage}");
                            _logger.LogError(
                                $"Endpoint:{context.Features.Get<IEndpointFeature>()?.Endpoint} Attribute:Default. Custom error message: {customErrorMessage}. Custom Response status: {(int)statusCode} " +
                                $"Exception message: {e.Message} " +
                                $"Inner exception: {e.InnerException?.Message ?? "N/a"}");
                            throw;
                        }
                        break;

                    case AttributeProperty.Faulted:
                        context.Response.StatusCode = 500;
                        _logger.LogError(
                            $"Endpoint:{context.Features.Get<IEndpointFeature>()?.Endpoint} Attribute.Faulted. Custom error message: {customErrorMessage ?? "Faulted API Endpoint"}. Custom Response status: {(int)statusCode}");
                        throw new Exception($"{customErrorMessage ?? "Faulted API Endpoint"}");

                    default:
                        context.Response.StatusCode = (int)statusCode;
                        break;
                }
            }
            else
            {
                /* move on when resilentr is not used */
                await _next(context);
            }
        }

        /// <summary>
        /// Get attribute value by using reflection.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private AttributeProperty? GetAttributeProperty(ResilentrAttribute attribute)
        {
            var field = typeof(ResilentrAttribute).GetField("_attribute", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field is not null)
            {
                return (AttributeProperty?)field.GetValue(attribute);
            }
            return null;
        }
        
        /// <summary>
        /// Get custom error message by using reflection.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private string? GetCustomErrorMessage(ResilentrAttribute attribute)
        {
            var field = typeof(ResilentrAttribute).GetField("_customErrorMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field is not null)
            {
                return (string?)field.GetValue(attribute);
            }
            return null;
        }

        /// <summary>
        /// Get custom HTTP Status code by using reflection.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private HttpStatusCode? GetHttpStatusCode(ResilentrAttribute attribute)
        {
            var field = typeof(ResilentrAttribute).GetField("_statusCode", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field is not null)
            {
                return (HttpStatusCode?)field.GetValue(attribute);
            }
            return null;
        }
    }
}
