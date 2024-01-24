using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Moq;
using Resilentr.src.Common;
using Resilentr.src.Resilentr.Client;
using Resilentr.src.Resilentr.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Resilentr.Test.Attributes
{
    public class LoudAttributeTest
    {
        [Fact]
        public async Task LoudAttribute()
        {
            var middleware = new ResilentrMiddleware(_ => throw new Exception("Loud Exception"));
            var context = new DefaultHttpContext();
            var endpointFeature = new Mock<IEndpointFeature>();
            string expectedSubstring = "some error message";
            var attribute = new ResilentrAttribute(AttributeProperty.Loud, expectedSubstring, HttpStatusCode.BadRequest);

            endpointFeature.Setup(x => x.Endpoint).Returns(new Endpoint(null, new EndpointMetadataCollection(attribute), null));
            context.Features.Set(endpointFeature.Object);

            await middleware.Invoke(context);

            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;
            await middleware.Invoke(context);

            Assert.Equal(400, context.Response.StatusCode);

            memoryStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);
            string responseContent = await reader.ReadToEndAsync();
            Assert.Contains(expectedSubstring, responseContent);
        }
    }
}
