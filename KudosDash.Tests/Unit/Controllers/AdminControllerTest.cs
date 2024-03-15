using FakeItEasy;
using FluentAssertions;
using KudosDash.Controllers;
using KudosDash.Data;
using KudosDash.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KudosDash.Tests.Unit
{
	[TestFixture]
	public class AdminControllerTest
	{

		private AdminController? _adminController;
		private ApplicationDbContext? _context;
		private SqliteConnection? sqliteConnection;

		[SetUp]
		public void SetUp()
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
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
			sqliteConnection.Close();
		}

		[Test]
		public void AdminController_Index_ReturnsSuccess()
		{
			// Arrange
			var mock = new Mock<ILogger<AdminController>>();
			ILogger<AdminController> logger = mock.Object;
			_adminController = new AdminController(_context, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_adminController.ControllerContext = controllerContext;

			// Act
			var result = _adminController.Index();

			// Assert
			result.Should().BeOfType<ViewResult>();
		}

		[Test]
		public void AdminController_Delete_ReturnsSuccess()
		{
			// Arrange
			var mock = new Mock<ILogger<AdminController>>();
			ILogger<AdminController> logger = mock.Object;
			_adminController = new AdminController(_context, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_adminController.ControllerContext = controllerContext;

			_context.Account.Add(new AppUser
			{
				Id = "test",
				FirstName = "test",
				LastName = "test",
				Email = "test@test.com",
				UserName = "test@test.com",
				TeamId = null
			});
			_context.SaveChanges();

			// Act
			var result = _adminController.Delete("test");

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			_context.Account.Count().Should().Be(1);
		}

		[Test]
		public void AdminController_DeleteMissingID_ReturnsFailure()
		{
			// Arrange
			var mock = new Mock<ILogger<AdminController>>();
			ILogger<AdminController> logger = mock.Object;
			_adminController = new AdminController(_context, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_adminController.ControllerContext = controllerContext;

			// Act
			var result = _adminController.Delete("test");
			var resultStatusCode = result.Result as StatusCodeResult;

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			resultStatusCode.StatusCode.Should().Be(404);
		}


		[Test]
		public void AdminController_DeleteConfirmed_ReturnsSuccess()
		{
			// Arrange
			var mock = new Mock<ILogger<AdminController>>();
			ILogger<AdminController> logger = mock.Object;
			_adminController = new AdminController(_context, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_adminController.ControllerContext = controllerContext;
			_adminController.TempData = A.Fake<TempDataDictionary>();

			_context.Account.Add(new AppUser
			{
				Id = "test",
				FirstName = "test",
				LastName = "test",
				Email = "test@test.com",
				UserName = "test@test.com",
				TeamId = null
			});
			_context.SaveChanges();
			var initialAccountCount = _context.Account.Count();

			// Act
			var result = _adminController.DeleteConfirmed("test");

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			_context.Account.Count().Should().NotBe(initialAccountCount);
		}
	}
}