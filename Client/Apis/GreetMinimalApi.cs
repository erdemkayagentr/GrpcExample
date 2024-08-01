using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Server;

namespace Client.Apis;

public static class GreetMinimalApi
{
    // Minimal API endpointlerini eklemek için extension metodu.
    public static void UseGreetEndPoints(this IEndpointRouteBuilder routes)
    {
        // Tüm greet endpointlerini gruplayarak organize eder.
        var group = routes.MapGroup("greet");

        // Unary gRPC çağrısını gerçekleştiren GET endpointi.
        group.MapGet("unary", UnaryExample).WithName("unary").WithOpenApi();

        // Server Streaming gRPC çağrısını gerçekleştiren GET endpointi.
        group.MapGet("serverstream", ServerStreamExample).WithName("serverstream").WithOpenApi();

        // Client Streaming gRPC çağrısını gerçekleştiren GET endpointi.
        group.MapGet("clientstream", ClientStreamExample).WithName("clientstream").WithOpenApi();

        // Bidirectional Streaming gRPC çağrısını gerçekleştiren GET endpointi.
        group.MapGet("bidirectionalstream", BidirectionalStreamExample).WithName("bidirectionalstream").WithOpenApi();
    }

    // Unary gRPC çağrısı örneği. İstemciden adı ve soyadı alır, sunucuya gönderir ve yanıtı döner.
    private static async Task<string> UnaryExample([FromServices] Greeter.GreeterClient grpClient, [FromQuery] string name, [FromQuery] string surname)
    {
        // Sunucuya bir HelloRequest mesajı gönderir ve yanıtı alır.
        var grpcResponse = await grpClient.SayHelloUnaryAsync(new HelloRequest { FirstName = name, LastName = surname });
        // Yanıt mesajını döner.
        return grpcResponse.Message;
    }

    // Server Streaming gRPC çağrısı örneği. İstemciden adı ve soyadı alır, sunucuya gönderir ve sunucudan birden fazla yanıt alır.
    private static async Task<string> ServerStreamExample([FromServices] Greeter.GreeterClient grpClient, [FromQuery] string name, [FromQuery] string surname)
    {
        // Sunucuya bir HelloRequest mesajı gönderir ve yanıt akışını alır.
        var grpcResponse = grpClient.SayHelloServerStream(new HelloRequest { FirstName = name, LastName = surname });
        var response = "";
        // Yanıt akışından tüm mesajları okur ve birleştirir.
        await foreach (var message in grpcResponse.ResponseStream.ReadAllAsync())
        {
            response += message.Message + "\n";
        }
        // Tüm mesajları döner.
        return response;
    }

    // Client Streaming gRPC çağrısı örneği. İstemciden adı ve soyadı alır, sunucuya birden fazla mesaj gönderir ve sunucudan yanıt alır.
    private static async Task<string> ClientStreamExample([FromServices] Greeter.GreeterClient grpClient, [FromQuery] string name, [FromQuery] string surname)
    {
        // Sunucuya mesaj akışı başlatır.
        var grpcResponse = grpClient.SayHelloClientStream();
        // İstemciden gelen mesajları sunucuya gönderir.
        await grpcResponse.RequestStream.WriteAsync(new HelloRequest { FirstName = name, LastName = surname });
        await grpcResponse.RequestStream.CompleteAsync();
        // Sunucudan gelen yanıtı döner.
        return (await grpcResponse).Message;
    }

    // Bidirectional Streaming gRPC çağrısı örneği. İstemciden adı ve soyadı alır, sunucuya ve istemciye sürekli mesaj gönderir ve alır.
    private static async Task<string> BidirectionalStreamExample([FromServices] Greeter.GreeterClient grpClient, [FromQuery] string name, [FromQuery] string surname)
    {
        // Sunucuya iki yönlü mesaj akışı başlatır.
        var grpcResponse = grpClient.SayHelloBidirectionalStream();
        // İstemciden gelen mesajları sunucuya gönderir.
        await grpcResponse.RequestStream.WriteAsync(new HelloRequest { FirstName = name, LastName = surname });
        await grpcResponse.RequestStream.CompleteAsync();
        var response = "";
        // Sunucudan gelen tüm mesajları okur ve birleştirir.
        await foreach (var message in grpcResponse.ResponseStream.ReadAllAsync())
        {
            response += message.Message + "\n";
        }
        // Tüm mesajları döner.
        return response;
    }
}