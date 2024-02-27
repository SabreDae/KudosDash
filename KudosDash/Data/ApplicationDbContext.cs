using KudosDash.Models;
using KudosDash.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KudosDash.Data
{
	public class ApplicationDbContext : IdentityDbContext<AppUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<AppUser> Account { get; set; }

		public DbSet<Feedback> Feedback { get; set; }

		public DbSet<Teams> Teams { get; set; }
		public DbSet<KudosDash.Models.Users.AccountVM> AccountVM { get; set; } = default!;
	}
}