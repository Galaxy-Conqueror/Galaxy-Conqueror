using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;

namespace Galaxy.Conqueror.Tests.Helpers;

public static class HttpMessageHandlerMockExtensions
{
    public static void SetupJsonResponse<T>(
        this Mock<HttpMessageHandler> handlerMock,
        HttpMethod method,
        string url,
        T responseBody)
    {
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method && req.RequestUri!.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(responseBody), Encoding.UTF8, "application/json")
            });
    }
}
