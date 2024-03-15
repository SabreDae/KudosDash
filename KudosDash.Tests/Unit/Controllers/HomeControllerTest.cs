using FluentAssertions;
using KudosDash.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace KudosDash.Tests.Unit
{
	[TestFixture]
	public class HomeControllerTests
	{
		private HomeController? _homeController;

		[Test]
		public void HomeController_View_ReturnsSuccess()
		{
			// Arrange
			_homeController = new HomeController();

			// Act
			var result = _homeController.Index();

			// Assert
			result.Should().BeOfType<ViewResult>();
		}
	}
}