using Blazor.Assembly.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blazor.Assembly.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped<SessionStorageService>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


            builder.Services.AddSingleton(sp =>
            {
                var hubConnection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:5001/chathub", options =>
                    {
                        options.Transports = HttpTransportType.LongPolling ; // Allow multiple transports
                    })
                    .WithAutomaticReconnect()
                    .Build();

                return hubConnection;
            });

            await builder.Build().RunAsync();
        }
    }
}
