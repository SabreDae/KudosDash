using System.Net;
using FakeItEasy;
using FluentAssertions;
using KudosDash.Controllers;
using KudosDash.Data;
using KudosDash.Models;
using KudosDash.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace KudosDash.Tests.Unit
{
	[TestFixture]
	public class TeamsControllerTests
	{
		private TeamsController? _teamsController;
		private AccountController? _accountController;
		private ApplicationDbContext? _context;
		private UserManager<AppUser>? _userManager;
		private SignInManager<AppUser>? _signInManager;
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

			// Setup mock user manager and sign in manager
			var store = new UserStore<AppUser>(_context);
			_userManager = TestUserManager<AppUser>(store);
			_signInManager = A.Fake<SignInManager<AppUser>>();


			IConfigurationRoot configuration = builder.Build();
		}


		public static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser>? store = null) where TUser : class
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

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
			sqliteConnection.Close();
		}

		[Test]
		public void TeamsController_Create_ReturnsSuccessful()
		{
			// Arrange
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);

			// Act
			var result = _teamsController.Create();

			// Assert
			result.Should().BeOfType<ViewResult>();
		}

		[Test]
		public void TeamsController_Create_NewTeam_ReturnsSuccessful()
		{
			// Arrange
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			// Act
			var result = _teamsController.Create(team);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			_context.Teams.Count().Should().Be(1);
		}

		[Test]
		public void TeamsController_Create_DuplicateTeam_ReturnsFailure()
		{
			// Arrange
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			// Create first team 
			_ = _teamsController.Create(team);

			// Act - create second team with same name
			var result = _teamsController.Create(new Teams() {TeamId = 2, TeamName = "Test"});

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			_teamsController.ModelState.IsValid.Should().Be(false);
			_context.Teams.Count().Should().Be(1);
		}

		[Test]
		public void TeamsController_Create_NewTeam_MissingField_ReturnsFailure()
		{
			// Arrange
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			var team = new Teams()
			{
				TeamId = 1,
			};

			// Act
			var result = _teamsController.Create(team);

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
			_context.Teams.Count().Should().Be(0);
		}

		[Test]
		public void TeamsController_Create_ManagerRole_ReturnsSuccess()
		{
			// Arrange
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> Tlogger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, Tlogger);
			var mockALogger = new Mock<ILogger<AccountController>>();
			ILogger<AccountController> logger = mockALogger.Object;
			_accountController = new AccountController(_signInManager, _userManager, _context, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Manager") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			// Create a user
			var testUser = new AppUser
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

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			// Act
			var result = _teamsController.Create(team);

			// Assert
			_context.Teams.Count().Should().Be(1);
		}

		[Test]
		public void TeamsController_Index_ReturnsSuccess()
		{
			// Arrange
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			// Act
			var result = _teamsController.Index();

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
		}

		[Test]
		public void TeamsController_Details_ReturnsSuccess()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			var create = _teamsController.Create(team);

			// Act
			var result = _teamsController.Details(1);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
		}

		[Test]
		public async Task TeamsController_DetailsNoRecord_ReturnsNotFound()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			// Act
			var result = await _teamsController.Details(2) as NotFoundResult;

			// Assert
			result.StatusCode.Should().Be(404);
		}

		[Test]
		public void TeamsController_DetailsNotTeamManager_ReturnsFailure()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Manager") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			var create = _teamsController.Create(team);

			// Act
			var result = _teamsController.Details(1);

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted); // No logged in user
		}


		[Test]
		public void TeamsController_Edit_ReturnsSuccess()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			var create = _teamsController.Create(team);

			// Act
			var result = _teamsController.Edit(1);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
		}

		[Test]
		public void TeamsController_EditNotTeamManager_ReturnsFailure()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Manager") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			var create = _teamsController.Create(team);

			// Act
			var result = _teamsController.Edit(1);

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
		}

		[Test]
		public async Task TeamsController_EditNoRecord_ReturnsNotFound()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			// Act
			var result = await _teamsController.Edit(2) as NotFoundResult;

			// Assert
			result.StatusCode.Should().Be(404);
		}

		[Test]
		public void TeamsController_Delete_ReturnsSuccess()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			var create = _teamsController.Create(team);

			// Act
			var result = _teamsController.Delete(1);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
		}

		[Test]
		public async Task TeamsController_DeleteNoRecord_ReturnsNotFound()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;

			// Act
			var result = await _teamsController.Delete(2) as NotFoundResult;

			// Assert
			result.StatusCode.Should().Be(404);
		}

		[Test]
		public void TeamsController_DeleteNotTeamManager_ReturnsFailure()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Manager") == true)
			};
			_teamsController.ControllerContext = controllerContext;
			_teamsController.TempData = A.Fake<TempDataDictionary>();

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			var create = _teamsController.Create(team);

			// Act
			var result = _teamsController.Delete(1);

			// Assert
			result.Status.Should().Be(TaskStatus.Faulted);
		}

		[Test]
		public void TeamsController_DeleteConfirmed_ReturnsSuccess()
		{
			var mock = new Mock<ILogger<TeamsController>>();
			ILogger<TeamsController> logger = mock.Object;
			_teamsController = new TeamsController(_context, _userManager, logger);
			var controllerContext = new ControllerContext()
			{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
			};
			_teamsController.ControllerContext = controllerContext;
			_teamsController.TempData = A.Fake<TempDataDictionary>();

			var team = new Teams()
			{
				TeamId = 1,
				TeamName = "Test"
			};

			var create = _teamsController.Create(team);

			// Act
			var result = _teamsController.DeleteConfirmed(1);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			_context.Teams.Count().Should().Be(0);
		}
	}
}