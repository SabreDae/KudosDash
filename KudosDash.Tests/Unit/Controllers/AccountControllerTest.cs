using FakeItEasy;
using FluentAssertions;
using KudosDash.Controllers;
using KudosDash.Data;
using KudosDash.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace KudosDash.Tests.Unit
	{
	[TestFixture]
	public class AccountControllerTests
		{

		private AccountController? _accountController;
		private SignInManager<AppUser>? _signInManager;
		private UserManager<AppUser>? _userManager;
		private ApplicationDbContext? _context;
		private SqliteConnection? sqliteConnection;

		[SetUp]
		public void SetUp ()
			{
			// Build service colection to create identity UserManager and RoleManager.           
			IServiceCollection? serviceCollection = new ServiceCollection();

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

			// Setup mock user manager and sign in manager
			var store = new UserStore<AppUser>(_context);
			_userManager = TestUserManager<AppUser>(store);
			_signInManager = A.Fake<SignInManager<AppUser>>();

			IConfigurationRoot? configuration = builder.Build();
			}

		[TearDown]
		public void TearDown ()
			{
			_context.Database.EnsureDeleted();
			_context.Dispose();
			sqliteConnection.Close();
			}

		public static UserManager<TUser> TestUserManager<TUser> (IUserStore<TUser>? store = null) where TUser : class
			// Mock user manager as done in official Identity Repo: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/test/Shared/MockHelpers.cs#L33
			{
			store ??= new Mock<IUserStore<TUser>>().Object;
			var options = new Mock<IOptions<IdentityOptions>>();
			var idOptions = new IdentityOptions();
			idOptions.Lockout.AllowedForNewUsers = false;
			options.Setup(o => o.Value).Returns(idOptions);
			var userValidators = new List<IUserValidator<TUser>>();
			var validator = new Mock<IUserValidator<TUser>>();
			userValidators.Add(validator.Object);
			var pwdValidators = new List<PasswordValidator<TUser>>
				{
				new()
				};
			var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
				userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(), null,
				new Mock<ILogger<UserManager<TUser>>>().Object);
			validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
				.Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
			return userManager;
			}

		[Test]
		public void AccountController_Register_ReturnsSuccess ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Act
			var result = _accountController.Register();

			// Assert
			result.Should().BeOfType<ViewResult>();
			}

		[Test]
		public void AccountController_Login_ReturnsSuccess ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Act
			var result = _accountController.Login();

			// Assert
			result.Should().BeOfType<ViewResult>();
			}

		[Test]
		public void AccountController_Register_NewUser_Success ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Simulate user input
			var testUser = new RegisterVM
				{
				FirstName = "Testing",
				LastName = "Test",
				TeamId = null,
				Role = "Admin",
				Email = "test@test.com",
				Password = "Test-1234",
				ConfirmPassword = "Test-1234"
				};

			// Act 
			_ = _accountController.Register(testUser);

			// Assert
			_context.Account.Count().Should().Be(1);
			}

		[Test]
		public void AccountController_Register_NewUser_UnconfirmedPassword ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);
			// Simulate user input
			var testUser = new RegisterVM
				{
				FirstName = "Test",
				LastName = "Test",
				TeamId = null,
				Role = "Admin",
				Email = "test@test.com",
				Password = "Test-1234"
				};
			var initialDbCount = _context.Account.Count();

			// Act 
			var result = _accountController.Register(testUser);
			var postResultDbCount = _context.Account.Count();

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
			postResultDbCount.Should().NotBe(initialDbCount);
			}

		[Test]
		public void AccountController_Login_UserSuccess ()
			{
			// Arrange
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(Enumerable.Empty<Claim>()));

			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Create new user
			var testUser = new AppUser()
				{
				FirstName = "Test",
				LastName = "Test",
				Email = "test@test.com",
				UserName = "test@test.com"
				};

			_userManager.CreateAsync(testUser, "Test-1234");

			var loginUser = new LoginVM()
				{
				Email = testUser.Email,
				Password = "Test-1234",
				RememberMe = false
				};

			// Act 
			var result = _accountController.Login(loginUser);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			A.CallTo(() => _signInManager.IsSignedIn(claimsPrincipal)).Returns(false);
			}

		[Test]
		public void AccountController_Details_NoUser_ReturnsFailure ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Act - Attempt to get Details page with no authenticated user logged in
			var result = _accountController.Details();

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
			}

		[Test]
		public void AccountController_Details_AuthUser_ReturnsSuccess ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Create new user
			var testUser = new AppUser()
				{
				FirstName = "Test",
				LastName = "Test",
				Email = "test@test.com",
				UserName = "test@test.com"
				};

			_userManager.CreateAsync(testUser, "Test-1234");

			var loginUser = new LoginVM()
				{
				Email = testUser.Email,
				Password = "Test-1234",
				RememberMe = false
				};
			_ = _accountController.Login(loginUser);
			// Ensure HttpContext is accessible
			_accountController.ControllerContext = new ControllerContext
				{
				HttpContext = new DefaultHttpContext()
				};


			// Act - Attempt to get Details page with authenticated user logged in
			var result = _accountController.Details();

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}

		[Test]
		public void AccountController_Details_Change_ReturnsSuccess ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Create new user
			var testUser = new AppUser()
				{
				FirstName = "Test",
				LastName = "Test",
				Email = "test@test.com",
				UserName = "test@test.com"
				};

			_userManager.CreateAsync(testUser, "Test-1234");

			var loginUser = new LoginVM()
				{
				Email = testUser.Email,
				Password = "Test-1234",
				RememberMe = false
				};

			_ = _accountController.Login(loginUser);
			// Ensure HttpContext is accessible
			_accountController.ControllerContext = new ControllerContext
				{
				HttpContext = new DefaultHttpContext()
				};

			var newUserDetails = new AccountVM()
				{
				FirstName = "Test1",
				LastName = "Test1",
				Email = "hello@hello.co.uk",
				TeamName = null
				};

			// Act - Attempt to get Details page with authenticated user logged in
			var result = _accountController.Details(newUserDetails);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			// Verify database count is still 1 user
			_context.Account.Count().Should().Be(1);
			// Verify database record is updated
			_userManager.FindByEmailAsync(newUserDetails.Email).Should().NotBeNull();
			}

		[Test]
		public void AccountController_Delete_NoUser_ReturnsFailure ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Act - Attempt to get Details page with no authenticated user logged in
			var result = _accountController.Delete();

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
			}

		[Test]
		public void AccountController_Delete_AuthUser_ReturnsSuccess ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Create new user
			var testUser = new AppUser()
				{
				FirstName = "Test",
				LastName = "Test",
				Email = "test@test.com",
				UserName = "test@test.com"
				};

			_userManager.CreateAsync(testUser, "Test-1234");

			var loginUser = new LoginVM()
				{
				Email = testUser.Email,
				Password = "Test-1234",
				RememberMe = false
				};

			_ = _accountController.Login(loginUser);
			// Ensure HttpContext is accessible
			_accountController.ControllerContext = new ControllerContext
				{
				HttpContext = new DefaultHttpContext()
				};


			// Act - Attempt to get Details page with authenticated user logged in
			var result = _accountController.Delete();

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}

		[Test]
		public void AccountController_ActionDelete_ReturnsSuccess ()
			{
			// Arrange
			_accountController = new AccountController(_signInManager, _userManager, _context);

			// Create new user
			var testUser = new AppUser()
				{
				FirstName = "Test",
				LastName = "Test",
				Email = "test@test.com",
				UserName = "test@test.com"
				};

			_userManager.CreateAsync(testUser, "Test-1234");

			var loginUser = new LoginVM()
				{
				Email = testUser.Email,
				Password = "Test-1234",
				RememberMe = false
				};

			_ = _accountController.Login(loginUser);
			// Ensure HttpContext is accessible
			_accountController.ControllerContext = new ControllerContext
				{
				HttpContext = new DefaultHttpContext()
				};

			// Act - Attempt to get Delete page with authenticated user logged in
			var result = _accountController.DeleteConfirmed();

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}
		}
	}