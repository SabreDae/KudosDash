using KudosDash.Data;
using KudosDash.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KudosDash.Controllers
	{
	[Authorize(Roles = "Admin")]
	public class AdminController (ApplicationDbContext context) : Controller
		{
		private readonly ApplicationDbContext _context = context;

		// GET: Admin
		public IActionResult Index ()
			{
			var model = new AdminVM
				{
				Users = _context.Users.ToList(),
				Teams = _context.Teams.ToList(),
				FeedbackCol = _context.Feedback.ToList(),
				};

			return View(model);
			}
		}
	}
