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
using NUnit.Framework.Internal;

namespace KudosDash.Tests.Unit
	{
	[TestFixture]
	public class FeedbackControllerTests
		{
		private FeedbackController? _feedbackController;
		private ApplicationDbContext? _context;
		private SqliteConnection? sqliteConnection;
		private UserManager<AppUser>? _userManager;
		private SignInManager<AppUser>? _signInManager;

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

			// Setup mock user manager and sign in manager
			var store = new UserStore<AppUser>(_context);
			_userManager = TestUserManager<AppUser>(store);
			_signInManager = A.Fake<SignInManager<AppUser>>();

			IConfigurationRoot configuration = builder.Build();
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

		[TearDown]
		public void TearDown ()
			{
			_context.Database.EnsureDeleted();
			_context.Dispose();
			sqliteConnection.Close();
			}

		[Test]
		public void FeedbackController_Index_ReturnsSuccess ()
			{
			// Arrange
			_feedbackController = new FeedbackController(_context, _userManager);
			var controllerContext = new ControllerContext()
				{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
				};
			_feedbackController.ControllerContext = controllerContext;

			// Act
			var result = _feedbackController.Index();

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}

		[Test]
		public void FeedbackController_Create_ReturnsSuccess ()
			{
			// Arrange
			_feedbackController = new FeedbackController(_context, _userManager);
			var controllerContext = new ControllerContext()
				{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
				};
			_feedbackController.ControllerContext = controllerContext;

			// Act
			var result = _feedbackController.Create();

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}

		[Test]
		public void FeedbackController_Details_ReturnsSuccess ()
			{
			// Arrange
			_feedbackController = new FeedbackController(_context, _userManager);
			var controllerContext = new ControllerContext()
				{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
				};
			_feedbackController.ControllerContext = controllerContext;

			// Act
			var result = _feedbackController.Details(1);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}

		[Test]
		public void FeedbackController_Edit_ReturnsSuccess ()
			{
			// Arrange
			_feedbackController = new FeedbackController(_context, _userManager);
			var controllerContext = new ControllerContext()
				{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
				};
			_feedbackController.ControllerContext = controllerContext;

			// Act
			var result = _feedbackController.Edit(1);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}

		[Test]
		public void FeedbackController_Delete_ReturnsSuccess ()
			{
			// Arrange
			_feedbackController = new FeedbackController(_context, _userManager);
			var controllerContext = new ControllerContext()
				{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Admin") == true)
				};
			_feedbackController.ControllerContext = controllerContext;

			// Act
			var result = _feedbackController.Delete(1);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}

		[Test]
		public void FeedbackController_ManagerApproved_ReturnsSuccess ()
			{
			// Arrange
			_feedbackController = new FeedbackController(_context, _userManager);
			var controllerContext = new ControllerContext()
				{
				HttpContext = Mock.Of<HttpContext>(ctx => ctx.User.IsInRole("Manager") == true)
				};
			_feedbackController.ControllerContext = controllerContext;

			// Act
			var result = _feedbackController.ManagerApproved(1);

			// Assert
			result.Status.Should().Be(TaskStatus.RanToCompletion);
			}
		}
	}