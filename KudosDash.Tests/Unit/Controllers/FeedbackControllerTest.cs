﻿using KudosDash.Controllers;
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

namespace KudosDash.Tests.Unit
{
	[TestFixture]
	public class FeedbackControllerTests
	{
		private FeedbackController _feedbackController;
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
		public void FeedbackController_Create_ReturnsSuccess()
		{
			// Arrange
			_feedbackController = new FeedbackController(_context);

			// Act
			var result = _feedbackController.Create();

			// Assert
			result.Should().BeOfType<ViewResult>();
		}
	}
}