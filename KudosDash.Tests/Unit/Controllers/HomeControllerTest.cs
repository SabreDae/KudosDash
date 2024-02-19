using FluentAssertions;
using KudosDash.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace KudosDash.Tests.Unit
	{
	[TestFixture]
	public class HomeControllerTests
		{
		private ILogger<HomeController> _logger;
		private HomeController _homeController;

		[Test]
		public void HomeController_View_ReturnsSuccess ()
			{
			// Arrange
			_homeController = new HomeController(_logger);

			// Act
			var result = _homeController.Index();

			// Assert
			result.Should().BeOfType<ViewResult>();
			}
		}
	}
