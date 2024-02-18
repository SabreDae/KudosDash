using KudosDash.Controllers;
using KudosDash.Data;
using KudosDash.Models.Users;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using KudosDash.Models;

namespace KudosDash.Tests.Unit
	{
	[TestFixture]
	public class TeamsControllerTests
		{
		private TeamsController _teamsController;
		private ApplicationDbContext _context;
		private SqliteConnection sqliteConnection;

		[SetUp]
		public void SetUp ()
			{
			// Build service colection to create identity UserManager and RoleManager.           
			IServiceCollection serviceCollection = new ServiceCollection();

			// Add ASP.NET Core Identity database in memory.
			sqliteConnection = new SqliteConnection("DataSource=:memory:");
			serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(sqliteConnection));

			_context = serviceCollection.BuildServiceProvider().GetService<ApplicationDbContext>();
			_context.Database.OpenConnection();
			_context.Database.EnsureCreated();

			// Add Identity using in memory database to create UserManager and RoleManager.
			serviceCollection.AddIdentity<AppUser, IdentityRole>()
				.AddUserManager<UserManager<AppUser>>()
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			IConfigurationRoot configuration = builder.Build();
			}

		[TearDown]
		public void TearDown ()
			{
			_context.Database.EnsureDeleted();
			_context.Dispose();
			sqliteConnection.Close();
			}

		[Test]
		public void TeamsController_Create_ReturnsSuccessful()
			{
			// Arrange
			_teamsController = new TeamsController(_context);

			// Act
			var result = _teamsController.Create();

			// Assert
			result.Should().BeOfType<ViewResult>();
			}

		[Test]
		public void TeamsController_Create_NewTeam_ReturnsSuccessful()
			{
			// Arrange
			_teamsController = new TeamsController(_context);
			var team = new Teams()
				{
				TeamId = 1,
				TeamName = "Test"
				};

			// Act
			var result = _teamsController.Create(team);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}

		[Test]
		public void TeamsController_Create_NewTeam_MissingField_ReturnsFailure()
			{
			_teamsController = new TeamsController(_context);
			var team = new Teams()
				{
				TeamId = 1,
				};

			// Act
			var result = _teamsController.Create(team);

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
			}

		[Test]
		public void TeamsController_Index_ReturnsSuccess()
			{
			// Arrange
			_teamsController = new TeamsController(_context);

			// Act
			var result = _teamsController.Index();

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}
		}
	}
