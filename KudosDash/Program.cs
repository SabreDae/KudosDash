using KudosDash.Data;
using KudosDash.Models.Users;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KudosDash
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Logging.ClearProviders();
			builder.Logging.AddConsole();

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddIdentity<AppUser, IdentityRole>(
				options =>
				{
					// FIXME: Confirmed Account set to false during development; set to true for production
					options.SignIn.RequireConfirmedAccount = false;
					// Password complexity rules
					options.Password.RequireUppercase = true;
					options.Password.RequireLowercase = true;
					options.Password.RequireDigit = true;
					options.Password.RequiredLength = 8;
					options.Password.RequireNonAlphanumeric = false;
					// Lockout settings
					options.Lockout.MaxFailedAccessAttempts = 10;
					options.Lockout.AllowedForNewUsers = true;
					options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(3);
				}
				)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();

			// Cookie settings for login session
			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.Name = ".AspNetCore.Identity.Application";
				options.ExpireTimeSpan = TimeSpan.FromMinutes(3);
				// After 3 minutes of inactivty, user will be auto-logged out on next request
				options.SlidingExpiration = true;
			});

			builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				//Log all errors in the application
				app.UseExceptionHandler(errorApp =>
				{
					errorApp.Run(async context =>
					{
						var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
						var exception = errorFeature.Error;

						Console.WriteLine(String.Format("Stacktrace of error: {0}", exception.StackTrace.ToString()));
					});
				});
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			/* HttpsRedirection is not configured here, as the hosting solution automatically redirects from HTTP to HTTPS. 
			Setting up redirection here also causes an infinite redirect loop for the deployed application.
			*/

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseSession();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				// Seed roles on App launch in every environment
				var roles = new string[] { "Admin", "Manager", "Team Member" };
				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
						// If role does not already exist, create it
						await roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			app.Run();
		}
	}
}