using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Client
{
    public class Startup
    {
        public const string TraceId = "X-B3-TraceId";
        public const string SpanId = "X-B3-SpanId";
        public const string ParentSpanId = "X-B3-ParentSpanId";
        public const string Sampled = "X-B3-Sampled"; // Will be replaced by Flags in the future releases of Finagle
        public const string Flags = "X-B3-Flags";
        public const string B3 = "b3";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("ShoppingAPIClient", client =>
            {
                //client.BaseAddress = new Uri("http://localhost:5000/"); // Shopping.API url     
                client.BaseAddress = new Uri(Configuration["ShoppingAPIUrl"]);
            }).AddHeaderPropagation();


            services.AddHeaderPropagation(o =>
            {
                // Propagate if header exists
                o.Headers.Add(TraceId);
                o.Headers.Add(SpanId);
                o.Headers.Add(ParentSpanId);
                o.Headers.Add(Sampled);
                o.Headers.Add(Flags);
                o.Headers.Add(B3);
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHeaderPropagation();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
