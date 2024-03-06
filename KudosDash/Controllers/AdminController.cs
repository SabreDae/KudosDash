using KudosDash.Data;
using KudosDash.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KudosDash.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController(ApplicationDbContext context, ILogger<AdminController> logger) : Controller
	{
		private readonly ApplicationDbContext _context = context;
		private readonly ILogger<AdminController> _logger = logger;

		// GET: Admin
		public IActionResult Index()
		// Admin dashboard allowing browsing of all database records
		{
			var model = new AdminVM
			{
				Users = _context.Users.ToList(),
				Teams = _context.Teams.ToList(),
				FeedbackCol = _context.Feedback.ToList(),
			};
			_logger.LogInformation("Admin Dashboard accessed at {DT}", DateTime.UtcNow);
			return View(model);
		}


		// GET: Admin/Delete/1
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		// Method to allow admin to delete another user's account. 
		{
			if (id == null)
			{
				return NotFound();
			}

			var account = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
			if (account == null)
			{
				return NotFound();
			}

			return View(account);
		}

		// POST: Admin/Delete/1
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			var account = await _context.Users.FindAsync(id);
			if (account != null)
			{
				_context.Users.Remove(account);
			}
			await _context.SaveChangesAsync();
			_logger.LogInformation("Admininistrator deleted account {Acc} at {DT}", id, DateTime.UtcNow);
			TempData["AlertMessage"] = "User account has successfully been deleted!";
			return RedirectToAction("Index");
		}
	}
}