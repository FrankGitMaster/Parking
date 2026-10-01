using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Parking.Infraestructura.Roles;
using Parking.Models;

namespace Parking
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ParkingDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("CadenaConexion")).UseSnakeCaseNamingConvention();
            });

            builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            }).AddEntityFrameworkStores<ParkingDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                //Seguridad
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                //Expiración
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                //Rutas personalizadas
                options.LoginPath = "/Cuenta/Login";
                options.LogoutPath = "/Cuenta/Logout";
                options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
            });

            // 1. REGISTRAR (AddScoped) - En Program.cs
            builder.Services.AddScoped<IRolesInitializer, RolesInitializer>();

            ExcelPackage.License.SetNonCommercialPersonal("NonComercialPersonalLicence");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //2. RESOLVER Y EJECUTAR(después de app = builder.Build())
            using (var scope = app.Services.CreateScope())// CREAR: Espacio de trabajo temporal(scope)
            {
                var rolesInitializer = scope.ServiceProvider.GetRequiredService<IRolesInitializer>();// RESOLVER: Necesito un IRolesInitializer AHORA
                await rolesInitializer.RolesInitializeAsync();// EJECUTAR: El código que crea roles y admin
            }// using -> DESTRUIR: El scope y todos los servicios Scoped

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Cuenta}/{action=Login}/{id?}");

            app.Run();
        }
    }
}