using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using Resilentr.src.Common;
using Resilentr.src.Resilentr.Client;
using Resilentr.src.Resilentr.Middleware;
using System.Net;

namespace Resilentr.Test.Attributes
{
    public class FaultedAttributeTest
    {
        [Fact]
        public async Task FaultedAttribute()
        {
            var middleware = new ResilentrMiddleware(_ => Task.CompletedTask);
            var context = new DefaultHttpContext();
            var endpointFeature = new Mock<IEndpointFeature>();
            string customExcptionMessage = "doesnt matter it works or not, just fault it.";
            var attribute = new ResilentrAttribute(AttributeProperty.Faulted, customExcptionMessage, HttpStatusCode.OK);

            endpointFeature.Setup(x => x.Endpoint).Returns(new Endpoint(null, new EndpointMetadataCollection(attribute), null));
            context.Features.Set(endpointFeature.Object);

            await Assert.ThrowsAsync<Exception>(() => middleware.Invoke(context));

            Assert.Equal(500, context.Response.StatusCode);
        }
    }
}
