using Application;
using Application.Helper;
using Infra.Persistence;
using Infra.Shared;
using Infrastructure.Authentication;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Extensions;
using WebApi.Middlewares;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;

                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddApplicationLayer(Configuration);
            services.AddPersistenceInfrastructure(Configuration);
            services.AddAuthenticationInfrastructure(Configuration);
            services.AddSharedInfrastructure(Configuration);
            services.AddSwaggerExtension(Configuration);
            services.AddHealthCheckExtension(Configuration);


            services.AddControllers()
                  .AddNewtonsoftJson(options =>
                  {
                      options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                      options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                      options.SerializerSettings.Converters.Add(new ForWebApiTrimmingConverter());
                  })
                  .AddJsonOptions(jsonOptions =>
                  {
                      jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
                      jsonOptions.JsonSerializerOptions.DictionaryKeyPolicy = null;
                  })
                  .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerExtension(Configuration);
            }
            else if (env.IsProduction())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts(opt => opt.MaxAge(365).IncludeSubdomains());
            }

            app.UseForwardedHeaders();

            app.UseHttpsRedirection();

            ApplyMapsterConfig();

            ApplySecurityHeaders(app);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseErrorHandlingMiddleware();

            app.UseRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/health");
            });
        }

        private static void ApplyMapsterConfig()
        {
            //Mapster Global Config
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);

            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            //register configs for the following assemblies
            var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Application")); //the namespace on where the IRegister implements

            TypeAdapterConfig.GlobalSettings.Scan(applicationAssembly.ToArray());
        }

        private static void ApplySecurityHeaders(IApplicationBuilder app)
        {
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());

            app.UseXfo(opt => opt.Deny());

            app.UseCsp(opt => opt
                              .BlockAllMixedContent()
                              .FrameAncestors(a => a.None())
                   );

            app.UseXContentTypeOptions();

            app.UseReferrerPolicy(opt => opt.NoReferrer());
        }
    }
}
