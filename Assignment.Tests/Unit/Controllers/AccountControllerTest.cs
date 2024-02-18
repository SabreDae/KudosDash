using Assignment.Controllers;
using Assignment.Data;
using Assignment.Models.Users;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Assignment.Tests.Unit
{
	[TestFixture]
	public class AccountControllerTests
	{

		private AccountController _accountController;
		private SignInManager<AppUser> _signInManager;
		private UserManager<AppUser> _userManager;
		private RoleManager<IdentityRole> _roleManager;
		private ApplicationDbContext _context;
		private SqliteConnection sqliteConnection;

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
		public void AccountController_Register_ReturnsSuccess()
		{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Act
			var result = _accountController.Register();

			// Assert
			result.Should().BeOfType<ViewResult>();
		}

		[Test]
		public void AccountController_Login_ReturnsSuccess()
		{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Act
			var result = _accountController.Login();

			// Assert
			result.Should().BeOfType<ViewResult>();

		}

		[Test]
		public void AccountController_Details_NoUser_ReturnsFailure()
		{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Act - Attempt to get Details page with no authenticated user logged in
			var result = _accountController.Details();

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
		}

		[Test]
		public void AccountController_Register_NewUser_Success()
		{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Simulate user input
			var testUser = new RegisterVM
			{
				FirstName = "Testing",
				LastName = "Test",
				TeamName = "Test",
				Role = "Admin",
				Email = "test@test.com",
				Password = "Test1234",
				ConfirmPassword = "Test1234"
			};

			// Act 
			var result = _accountController.Register(testUser);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
		}

		[Test, Order(1)]
		// Flaky test - only passes if run before the above Register test
		public void AccountController_Register_NewUser_UnconfirmedPassword()
		{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);
			// Simulate user input
			var testUser = new RegisterVM
			{
				FirstName = "Test",
				LastName = "Test",
				TeamName = "Test",
				Role = "Admin",
				Email = "test@test.com",
				Password = "Test1234"
			};

			// Act 
			var result = _accountController.Register(testUser);

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
		}

		[Test]
		public void AccountController_Login_UserSuccess()
		{
			// Arrange
			_userManager = A.Fake<UserManager<AppUser>>();
			_signInManager = A.Fake<SignInManager<AppUser>>();
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Create new user
			var testUser = new AppUser()
			{
				FirstName = "Test",
				LastName = "Test",
				Role = "Admin",
				Email = "test@test.com",
				UserName = "test@test.com"
			};

			_userManager.CreateAsync(testUser, "Test1234");

			var loginUser = new LoginVM()
			{
				Email = testUser.Email,
				Password = "Test1234",
				RememberMe = false
			};

			// Act 
			var result = _accountController.Login(loginUser);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
		}
	}
}