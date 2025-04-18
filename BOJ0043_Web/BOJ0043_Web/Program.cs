using BOJ0043_Web.Data;
using BOJ0043_Web.Models;
using BOJ0043_Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BOJ0043_Web
{
    public class Program
    {
        public static void Main(string[] args)
        {            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            // Přidání podpory pro API s JSON formátováním
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
            });
            
            // Konfigurace CORS pro komunikaci s desktopovou aplikací
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            
            // Konfigurace SQLite databáze
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            // Konfigurace ASP.NET Core Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
                {
                    // Nastavení požadavků na heslo
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                    
                    // Nastavení potvrzení emailu
                    options.SignIn.RequireConfirmedEmail = false;
                    
                    // Nastavení zamykání účtu při příliš mnoha neúspěšných pokusech
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI(); // Přidá výchozí UI pro Identity (přihlašování, registrace, atd.)
                  
            // Přidání autorizace
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireReadOnlyRole", policy => policy.RequireRole("ReadOnly", "Admin"));
            });
                  
            // Register IActionContextAccessor for API Documentation UI
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            builder.Services.AddHttpClient();
            
            // Nastavení invariantní kultury pro model binding
            builder.Services.AddMvc()
                .AddMvcOptions(options =>
                {
                    // Přidání vlastního model binderu pro decimální hodnoty
                    options.ModelBinderProviders.Insert(0, new Infrastructure.DecimalModelBinderProvider());
                    
                    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
                        (x) => $"Hodnota '{x}' není platná.");
                })
                .AddDataAnnotationsLocalization();

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new System.Globalization.CultureInfo("en-US") };
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            
            // Registrace repozitářů
            builder.Services.AddScoped<ICoworkingSpaceRepository, CoworkingSpaceRepository>();
            builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            // Povolení CORS
            app.UseCors("AllowLocalhost");

            // Aplikace nastavení kultury/lokalizace
            app.UseRequestLocalization();

            // Přidání middleware pro autentizaci a autorizaci
            app.UseAuthentication();
            app.UseAuthorization();
              app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
                
            // Mapování API endpointů
            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller=Home}/{action=Index}/{id?}");
                
            // Mapování Identity stránek (přihlášení, registrace, atd.)
            app.MapRazorPages();

            // Zajistíme, že se databáze vytvoří a aplikují se migrace při spuštění aplikace
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    
                    // Inicializace rolí a admin účtu
                    SeedRoles(services).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Došlo k chybě při inicializaci databáze.");
                }
            }

            app.Run();
        }
        
        /// <summary>
        /// Inicializuje výchozí role a administrátorský účet
        /// </summary>
        private static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            // Vytvoření rolí, pokud ještě neexistují
            string[] roleNames = { "Admin", "ReadOnly" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            
            // Vytvoření výchozího admin účtu
            var adminEmail = "admin@coworking.cz";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "System",
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
