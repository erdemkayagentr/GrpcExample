using Grpc.Core;

namespace Server.Services;

public class GreetGrpcService : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHelloUnary(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = request.FirstName + " " + request.LastName + " from Unary"
        });
    }



    public override async Task SayHelloServerStream(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        int i = 0;
        while (!context.CancellationToken.IsCancellationRequested && i<10)
        {
            await responseStream.WriteAsync(new HelloReply
            {
                Message = request.FirstName + " " + request.LastName + " from Server Streaming"
            });
            i++;
        }
    }


    public override async Task<HelloReply> SayHelloClientStream(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
    {
        var names = new List<string>();
        await foreach (var message in requestStream.ReadAllAsync())
        {
            names.Add(message.FirstName + " " + message.LastName);
        }
        return new HelloReply
        {
            Message = string.Join(", ", names) + " from Client Streaming"
        };
    }

    public override async Task SayHelloBidirectionalStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream,
        ServerCallContext context)
    {
        await foreach (var message in requestStream.ReadAllAsync())
        {
            await responseStream.WriteAsync(new HelloReply
            {
                Message = message.FirstName + " " + message.LastName + " from Bidirectional Streaming"
            });
        }
    }

}
