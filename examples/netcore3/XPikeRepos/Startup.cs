using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XPike.DataStores.MySql.Pomelo;
using XPike.IoC.Microsoft.AspNetCore;
using XPike.Logging.Microsoft.AspNetCore;
using XPike.Settings.AspNetCore;

namespace XPikeRepos
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddXPikeSettings()
                .AddXPikeLogging()
                .AddXPikeDependencyInjection()
                .AddXPikeRepos()
                .AddXPikePomeloMySqlDataStores();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseXPikeLogging()
                .UseXPikeDependencyInjection();
                //.Verify();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}