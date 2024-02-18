using Assignment.Models;
using Assignment.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Data
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
	}
}