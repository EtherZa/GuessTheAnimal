namespace GuessTheAnimal.Web
{
    using GuessTheAnimal.Contracts.Config;
    using GuessTheAnimal.Contracts.Repository;
    using GuessTheAnimal.Contracts.Service;
    using GuessTheAnimal.Core;
    using GuessTheAnimal.Core.Config;
    using GuessTheAnimal.Data;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                                                    .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(
                routes => { routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}"); });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddOptions();
            services.Configure<ApplicationConfig>(this.Configuration);

            services.AddSingleton<IApplicationConfig>(
                x => x.GetService<IOptions<ApplicationConfig>>()
                      .Value);

            services.AddSingleton<IAnimalRepository, SqlAnimalRepository>();
            services.AddSingleton<IAnimalService, AnimalService>();
            services.AddSingleton<IContextTokenizer, ContextTokenizer>();
            services.AddSingleton<IGameService, GameService>();
        }
    }
}