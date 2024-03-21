using Duende.IdentityServer.Services;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                if (builder.Environment.IsEnvironment("docker"))
                {
                    options.IssuerUri = "identity-svc";
                }

            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients(builder.Configuration))
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<CustomProfileService>();

        builder.Services.ConfigureApplicationCookie(options => {
            options.Cookie.SameSite=SameSiteMode.Lax;
        });
        
        builder.Services.AddAuthentication();

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();

        if (app.Environment.IsProduction()) 
        {
            //for Making sure the service is using HTTPS this is required for the deployment 
            //in Kubernetes
            app.Use(async (ctx, next) =>
            {
                var serverUrls = ctx.RequestServices.GetRequiredService<IServerUrls>();
                serverUrls.Origin = serverUrls.Origin = "https://id.sephsphere.com";
                await next();
            });
        }
        
        app.UseIdentityServer();
        app.UseAuthorization();
        
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}