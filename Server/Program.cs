using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

//ConfigurationKestrel
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 5858); //http trafiğini için
    
    options.Listen(IPAddress.Any, 5859, listenOptions => //gRPC trafiğini dinleyecek
    {
        listenOptions.Protocols = HttpProtocols.Http2; //gRPC için HTTP/2 protokolünü kullanacağız
       // listenOptions.UseHttps("certificate.pfx", "password"); //https yapılandırması gerekirse..
    });
});
// Add services to the container.
builder.Services.AddGrpc();//gRPC hizmetlerini DI (Dependency Injection) konteynerine ekler.

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreetGrpcService>(); //GreetGrpcService adlı gRPC hizmetini HTTP istek hattına (pipeline) ekler. Bu hizmet, gRPC isteklerini karşılar.

app.MapGet("/protos/greet-protobuf", async httpContext => ///greet-protobuf endpointi yardımıyla Protobuf dosyasımızı istemcilere sunacağız.
{ //Bu kısımı generic hale getirip belli bir klasöraltındaki tüm proto dosyalarını sunacak şekilde değiştirebiliriz. Bu kısmı hayal gücünüze bırakıyorum. :)
    //destek gerekirse mail atabilirsiniz. :)
    httpContext.Response.ContentType = "text/plain;charset=utf-8";
    using var fs = File.OpenRead("Protos/greet.proto");
    using var sr = new StreamReader(fs, System.Text.Encoding.UTF8);
    while (!sr.EndOfStream)
    {
       var  protoLine = await sr.ReadLineAsync();
       if(protoLine != "/*>>" || protoLine != "<<*/")
       {
           await httpContext.Response.WriteAsync(protoLine);
       }
       
    }
});
app.Run();

