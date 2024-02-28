using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenTelemetry()
    .WithTracing(options =>
    {
        options
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddProcessor(new MyCustomProcessor())
            .AddConsoleExporter();
    });

var app = builder.Build();
app.MapGet("/weatherforecast", () => { return "Hello World"; });
app.Run();

class MyCustomProcessor: BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        string? httpStatusCode = null;
        foreach (var kvp in data.Tags)
        {
            if (kvp.Equals("http.response.status_code")) {
                httpStatusCode = kvp.Value;
                Console.WriteLine("FOUND IT!");
            }
        }
        if (httpStatusCode == null) {
            Console.WriteLine("NOT found it!");
        }
    }
}