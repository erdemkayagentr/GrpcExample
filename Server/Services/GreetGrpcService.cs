using Grpc.Core;

namespace Server.Services;

public class GreetGrpcService : Greeter.GreeterBase // Greeter.GreeterBase sınıfından türeyen GreetGrpcService sınıfı.
{
    // Unary gRPC çağrısını gerçekleştiren metot.
    public override Task<HelloReply> SayHelloUnary(HelloRequest request, ServerCallContext context)
    {
        // HelloReply mesajı oluşturur ve döner. İstemciden gelen ad ve soyadı birleştirir ve mesaj olarak döner.
        return Task.FromResult(new HelloReply
        {
            Message = request.FirstName + " " + request.LastName + " from Unary"
        });
    }

    // Server Streaming gRPC çağrısını gerçekleştiren metot.
    public override async Task SayHelloServerStream(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        int i = 0; // Döngü sayacını başlatır.
        while (!context.CancellationToken.IsCancellationRequested && i < 10) // İptal edilmediği sürece ve sayaç 10'dan küçük olduğu sürece döngü devam eder.
        {
            // Sunucuya her bir döngüde yanıt mesajı gönderir.
            await responseStream.WriteAsync(new HelloReply
            {
                Message = request.FirstName + " " + request.LastName + " from Server Streaming"
            });
            i++; // Sayaç artırılır.
        }
    }

    // Client Streaming gRPC çağrısını gerçekleştiren metot.
    public override async Task<HelloReply> SayHelloClientStream(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
    {
        var names = new List<string>(); // İstemciden gelen ad ve soyadları tutacak bir liste oluşturur.
        await foreach (var message in requestStream.ReadAllAsync()) // İstemciden gelen tüm mesajları okur.
        {
            // Her bir mesajdan adı ve soyadı alıp listeye ekler.
            names.Add(message.FirstName + " " + message.LastName);
        }
        // Tüm adları birleştirir ve yanıt olarak döner.
        return new HelloReply
        {
            Message = string.Join(", ", names) + " from Client Streaming"
        };
    }

    // Bidirectional Streaming gRPC çağrısını gerçekleştiren metot.
    public override async Task SayHelloBidirectionalStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream,
        ServerCallContext context)
    {
        await foreach (var message in requestStream.ReadAllAsync()) // İstemciden gelen tüm mesajları okur.
        {
            // Her bir mesaj için sunucuya yanıt mesajı gönderir.
            await responseStream.WriteAsync(new HelloReply
            {
                Message = message.FirstName + " " + message.LastName + " from Bidirectional Streaming"
            });
        }
    }
}
