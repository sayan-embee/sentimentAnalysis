using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using NSSOperationAutomationApp.Bots;
using NSSOperationAutomationApp.ServiceMethods;
using NSSOperationAutomationApp.ServiceMethods.MSGraphProvider;
using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using System.Reflection.Metadata;
using NSwag;
using Microsoft.Bot.Streaming.Payloads;
using NSwag.Generation.Processors.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;

namespace NSSOperationAutomationApp
{
    public class Startup
    {
        public IConfiguration _configuration;
       
        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(_configuration.GetSection("AzureAd"));

            services.RegisterConfigurationSettings(_configuration);

            services.AddHttpClient();

            services.AddControllers();

            services.AddMvc().AddMvcOptions(mvcopt => { mvcopt.EnableEndpointRouting = false; });

            services.AddOpenApiDocument((config) => 
            { 
                config.Title = "NSSOperationAutomationApp";
                //config.AddSecurity("Bearer", new OpenApiSecurityScheme
                //{
                //    In = OpenApiSecurityApiKeyLocation.Header,
                //    Name = "Authorization",
                //    Type = OpenApiSecuritySchemeType.Http,
                //    BearerFormat = "JWT",
                //    Scheme = "bearer"
                //});
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
                config.AddSecurity("Bearer", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                config.PostProcess = (document) =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "NSSOperationAutomationApp";
                    document.Info.Description = "NSSOperationAutomationApp in .Net 6.0";
                };

            });

            //services.RegisterConfigurationSettings(_configuration);

            services.RegisterAuthenticationServices(this._configuration);

            services.AddSingleton<TelemetryClient>();

            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>(); // OPTIONAL

            ////services.AddSingleton<ConfigurationBotFrameworkAuthentication, ConfigurationCredentialProvider>();
            ///
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>(); // OPTIONAL

            ////services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
            ////services.AddSingleton<ConfigurationBotFrameworkAuthentication>();
            ///
            services.AddSingleton<CommonBotFilterMiddleware>(); // OPTIONAL

            services.AddSingleton<CommonBotAdapter>(); // OPTIONAL

            services.AddTransient<IAppLifeCycleHandler, AppLifeCycleHandler>();

            services.AddScoped<AdminActivityHandler>();

            services.AddScoped<UserActivityHandler>();

            services.AddTransient<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>(); // OPTIONAL

            services.AddTransient<IGraphServiceClientProvider, GraphServiceClientProvider>();

            services.AddTransient<IUsersService, UsersService>();

            services.AddTransient<IGroupsService, GroupsService>();

            services.AddTransient<IOpenAIServices, OpenAIServices>();

            services.AddMemoryCache();

            services.RegisterDataServices();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Add OpenAPI 3.0 document serving middleware
                // Available at: http://localhost:<port>/swagger/v1/swagger.json
                app.UseOpenApi();

                // Add web UIs to interact with the document
                // Available at: http://localhost:<port>/swagger
                app.UseSwaggerUi3();
    
            }

            //app.UseRouting();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseCors(
                options => options.WithOrigins("*").AllowAnyHeader().AllowAnyOrigin()
            );

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSpaStaticFiles();

            app.UseMvc();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseBotFramework();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

        }
    }    
}
