using Presentation.gRPC.Services;
using Application; 
using Infrastructure;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplication();                              
builder.Services.AddInfrastructure(builder.Configuration);     

builder.Services.AddGrpc(opts =>
{
    opts.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

var app = builder.Build();

app.MapGrpcService<NewsGrpcService>();

app.MapGet("/", () => "gRPC Server đang chạy");

app.Run();