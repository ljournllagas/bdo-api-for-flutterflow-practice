using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

namespace WebApi
{
    public class Program
    {
        private static string GetSerilogFileConfig(IConfigurationRoot configuration, string customFilename, bool isCustom = false, string customFolder = "Custom")
        {
            string fileName = $"{configuration["Serilog:Properties:ApplicationName"]}_{customFilename}_{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}_";

            string logDirectory = isCustom
                ? $"{configuration["Serilog:Properties:TextFileLogDir"]}\\{customFolder}\\"
                : $"{configuration["Serilog:Properties:TextFileLogDir"]}";

            //create the logging directory, if it does not exists
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            return $"{logDirectory}{fileName}.txt";
        }

        private static void ConfigureLogging(IConfigurationRoot configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Logger(a =>
                    a.Filter.ByIncludingOnly(a => a.Equals("@l='Information'"))
                     .WriteTo.File(GetSerilogFileConfig(configuration, "Information"), rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 5_242_880, rollOnFileSizeLimit: true,
                            outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj} {Properties:j}")
                )
                .WriteTo.Logger(a =>
                    a.Filter.ByIncludingOnly(a => a.Equals("@l = 'Error'"))
                     .WriteTo.File(GetSerilogFileConfig(configuration, "Error"), rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 5_242_880, rollOnFileSizeLimit: true)
                )
                .WriteTo.Logger(a =>
                    a.Filter.ByIncludingOnly(a => a.Equals("@l = 'Warning'"))
                     .WriteTo.File(GetSerilogFileConfig(configuration, "Warning"), rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 5_242_880, rollOnFileSizeLimit: true)
                )
                .WriteTo.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Setting up the logger..");
        }

        public static async Task Main(string[] args)
        {
            try
            {
                Log.Information("Starting the application..");

                //read appsettings config and environment variables
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .Build();

                //config serilog
                ConfigureLogging(configuration);

                Log.Information("Creating API Host..");

                //create api host
                await CreateHostAsync(args, configuration);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task CreateHostAsync(string[] args, IConfigurationRoot configuration)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                if (Convert.ToBoolean(configuration.GetSection("UseAutoMigrateDb").Value))
                {
                    using var scope = host.Services.CreateScope();

                    var services = scope.ServiceProvider;

                    var context = services.GetRequiredService<ApplicationDbContext>();

                    context.Database.Migrate();
                }

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}");
                Log.Fatal("Error Message: " + ex.Message);
                Log.Fatal("Source: " + ex.Source);
                throw;
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
              Host.CreateDefaultBuilder(args)
              .UseSerilog()
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>()
                            .CaptureStartupErrors(true)
                            .ConfigureKestrel(x =>
                            {
                                x.AddServerHeader = false;
                            });
              });
    }
}
