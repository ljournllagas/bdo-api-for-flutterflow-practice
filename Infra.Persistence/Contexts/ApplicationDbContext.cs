using Application.Interfaces;
using Infra.Persistence.FluentEntityConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infra.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDateTimeService? _dateTime;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _dateTime = dateTime;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(CoreEventId.InvalidIncludePathError));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //all Decimals will have 18,6 Range
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,6)");
            }

            builder.ApplyConfiguration(new AuthConfig());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public class ApplicationDbFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            private readonly IDateTimeService? _dateTime;
            private readonly IHttpContextAccessor? _httpContextAccessor;

            public ApplicationDbFactory()
            {
                var configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                  .AddEnvironmentVariables();

                var root = configuration.Build();

                _connectionString = root.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            }

            public ApplicationDbFactory(IDateTimeService dateTime, IHttpContextAccessor httpContextAccessor)
            {
                _dateTime = dateTime;
                _httpContextAccessor = httpContextAccessor;
            }


            public readonly string _connectionString = string.Empty;


            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

                optionsBuilder.UseSqlServer(_connectionString);

                //    services.AddDbContext<ApplicationDbContext>(options =>
                //    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                //                      b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                //    .EnableDetailedErrors()
                //    , ServiceLifetime.Scoped
                //);

                return new ApplicationDbContext(optionsBuilder.Options, _dateTime!, _httpContextAccessor!);
            }

        }
    }
}
