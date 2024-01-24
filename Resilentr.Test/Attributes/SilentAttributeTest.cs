using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Moq;
using Resilentr.src.Common;
using Resilentr.src.Resilentr.Client;
using Resilentr.src.Resilentr.Middleware;
using System.Net;

namespace Resilentr.Test.Attributes
{
    public class SilentAttributeTest
    {
        [Fact]
        public async Task SilentAttribute()
        {
            var middleware = new ResilentrMiddleware(_ => Task.CompletedTask);
            var context = new DefaultHttpContext();
            var endpointFeature = new Mock<IEndpointFeature>();
            var attribute = new ResilentrAttribute(AttributeProperty.Silent, "some error messages", HttpStatusCode.BadRequest);

            endpointFeature.Setup(x => x.Endpoint).Returns(new Endpoint(null, new EndpointMetadataCollection(attribute), null));
            context.Features.Set(endpointFeature.Object);

            await middleware.Invoke(context);

            Assert.Equal(200, context.Response.StatusCode);
        }
    }
}
