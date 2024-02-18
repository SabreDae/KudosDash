using Assignment.Data;
using Assignment.Models.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Assignment
{
	public class Program
		{
		public static async Task Main (string[] args)
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
				options => {
					// FIXME: Confirmed Account set to false during development; set to true for production
					options.SignIn.RequireConfirmedAccount = false;
					// Password complexity rules
					options.Password.RequireUppercase = true;
					options.Password.RequireLowercase = true;
					options.Password.RequireDigit = true;
					options.Password.RequiredLength = 8;
					options.Password.RequireNonAlphanumeric = false;
				}
				)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();

			// Cookie settings
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
			{
				options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
				options.SlidingExpiration = true;
				options.AccessDeniedPath = "/Forbidden/";
			});

			builder.Services.AddHttpsRedirection(options =>
			{
				options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
				options.HttpsPort = 443;
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
				{
				app.UseMigrationsEndPoint();
				}
			else
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
			//using (var scope = app.Services.CreateScope())
			//	{
			//	/* 
			//	 On App launch, check for whether the Admin account exists in the database.
			//	 If it does not, create the required account and give it the Admin role.
			//	*/
			//	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
			//	string adminEmail = "s.e.forsyth@hotmail.co.uk";
			//	var user = await userManager.FindByEmailAsync(adminEmail);
			//	if (user != null)
			//		{
			//		Console.WriteLine("user exists");
			//		await userManager.AddToRoleAsync(user, "Admin");
			//		}
			//	}

			app.Run();
			}
		}
	}

