using Microsoft.Extensions.Hosting;

namespace Orleans.Silo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
               .UseOrleans(siloBuilder =>
               {
                   siloBuilder.AddAdoNetGrainStorage("Default", options =>
                   {
                       options.Invariant = "System.Data.SqlClient";
                       options.ConnectionString = "Server=.;Database=OrleansClusterDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true;";
                   });
                   siloBuilder.UseLocalhostClustering();
                   siloBuilder.UseDashboard();
               });

            using IHost host = builder.Build();


            await host.RunAsync();

            Console.WriteLine("Silo server has started");
            Console.ReadLine();

            await host.StopAsync();
        }
    }
}
