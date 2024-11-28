using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ProyectSoftware.Web.Data;
using ProyectSoftware.Web.Data.Entities;
using ProyectSoftware.Web.Data.Seeders;
using ProyectSoftware.Web.Helpers;
using ProyectSoftware.Web.Services;

namespace ProyectSoftware.Web
{
    public static class CustomConfiguration
    {
        public static WebApplicationBuilder AddCustomBuilderConfiguration(this WebApplicationBuilder builder)
        {

            // Data Context
            builder.Services.AddDbContext<DataContext>(conf =>
            {
                conf.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"));// Configura Entity Framework Core para usar SQL Server como proveedor de base de datos y obtiene la cadena de conexión desde la configuración de la aplicación.
            });
            builder.Services.AddHttpContextAccessor();
            AddServices(builder);

            // Identity and Access Managnet
            AddIAM(builder);


            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 10; // Configura la duración de las notificaciones a 10 segundos.
                config.IsDismissable = true; // Permite que las notificaciones sean descartables por el usuario.
                config.Position = NotyfPosition.BottomRight; // Configura la posición de las notificaciones en la esquina inferior derecha.
            });

            return builder;
        }

        private static void AddIAM(WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, IdentityRole>(x =>
            {
                x.User.RequireUniqueEmail = true;
                x.Password.RequireDigit = false;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequiredLength = 4;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "Auth";
                options.LoginPath = "/Account/Login"; // Ruta de inicio de sesión
                options.AccessDeniedPath = "/Account/NotAuthorized"; // Ruta de acceso denegado
            });

            builder.Services.AddAuthorization();
        }
        private static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRolesService, RolesService>();
            builder.Services.AddScoped<IGenderTypesService, GenderTypeServices>();
            builder.Services.AddTransient<SeedDb>();
            builder.Services.AddScoped<IUsersService, UsersServices>();

            //helpers
            builder.Services.AddScoped<IConverterHelper, ConverterHelper>();
            builder.Services.AddScoped<ICombosHelper, CombosHelper>();
        }

        public static WebApplication AddCustomConfiguration(this WebApplication app)
        {
            app.UseNotyf();

            SeedData(app);

            return app;
        }


        private static void SeedData(WebApplication app)
        {
            IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>();

            using (IServiceScope scope = scopedFactory!.CreateScope())
            {
                SeedDb service = scope.ServiceProvider.GetService<SeedDb>();
                service!.SeedAsync().Wait();
            }
        }
    }
}
