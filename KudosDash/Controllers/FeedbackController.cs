using KudosDash.Data;
using KudosDash.Models;
using KudosDash.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KudosDash.Controllers
	{
	// Ensure only logged in users are able to access Feedback views/model
	[Authorize]
	public class FeedbackController : Controller
		{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public FeedbackController (ApplicationDbContext context, UserManager<AppUser> userManager)
			{
			_context = context;
			_userManager = userManager;
			}

		// GET: Feedback Index page view
		public async Task<IActionResult> Index ()
			{
			var currentUser = _userManager.GetUserId(User);
			if (User.IsInRole("Admin"))
				{
				return View(await _context.Feedback.ToListAsync());
				}
			else
				{
				if (User.IsInRole("Team Manager"))
					{
					/* TODO: Await a list of Feedback entries where the value of target user in the User table 
					has the same teamId in the User table as the teamId of the currently logged in user */
					return View();
					}
				return View(await _context.Feedback.Where(f => f.TargetUser == currentUser).ToListAsync());
				}

			}

		// GET: Feedback/Details/5
		public async Task<IActionResult> Details (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var feedback = await _context.Feedback
				.FirstOrDefaultAsync(m => m.ID == id);
			if (feedback == null)
				{
				return NotFound();
				}

			return View(feedback);
			}

		// GET: Feedback/Create
		public async Task<IActionResult> Create ()
			{
			var currentUser = await _userManager.GetUserAsync(User);
			var users = _context.Users.GroupBy(u => u.TeamId == currentUser.TeamId);
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			foreach (var user in users)
				{
				foreach (var item in user)
					{
					// Ensure user will not be able to select themselves in Target User dropdown
					if (item.Id != currentUser.Id)
						{
						selectListItems.Add(new SelectListItem() { Value = item.Id, Text = item.FirstName + " " + item.LastName });
						}
					}
				}
			ViewBag.Team = selectListItems;
			// Set date by default to current date
			Feedback model = new Feedback();
			model.FeedbackDate = DateTime.Now;
			return View(model);
			}

		// POST: Feedback/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create ([Bind("ID,Author,TargetUser,FeedbackDate,FeedbackText")] Feedback feedback)
			{
			var currentUser = await _userManager.GetUserAsync(User);
			if (ModelState.IsValid)
				{
				// The view does not include the author field, this should be set programmatically to the logged in user in the back-end
				feedback.Author = currentUser.Id;
				_context.Add(feedback);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
				}
			return View(feedback);
			}

		// GET: Feedback/Edit/5
		public async Task<IActionResult> Edit (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var feedback = await _context.Feedback.FindAsync(id);
			if (feedback == null)
				{
				return NotFound();
				}
			return View(feedback);
			}

		// POST: Feedback/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit (int id, [Bind("ID,Author,TargetUser,FeedbackDate,FeedbackText")] Feedback feedback)
			{
			if (id != feedback.ID)
				{
				return NotFound();
				}

			if (ModelState.IsValid)
				{
				try
					{
					_context.Update(feedback);
					await _context.SaveChangesAsync();
					}
				catch (DbUpdateConcurrencyException)
					{
					if (!FeedbackExists(feedback.ID))
						{
						return NotFound();
						}
					else
						{
						throw;
						}
					}
				return RedirectToAction(nameof(Index));
				}
			return View(feedback);
			}

		// GET: Feedback/Delete/5
		[Authorize(Roles = "Admin,Team Manager")]
		public async Task<IActionResult> Delete (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var feedback = await _context.Feedback
				.FirstOrDefaultAsync(m => m.ID == id);
			if (feedback == null)
				{
				return NotFound();
				}

			return View(feedback);
			}

		// POST: Feedback/Delete/5
		[HttpPost, ActionName("Delete")]
		[Authorize(Roles = "Admin,Team Manager")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed (int id)
			{
			var feedback = await _context.Feedback.FindAsync(id);
			if (feedback != null)
				{
				_context.Feedback.Remove(feedback);
				}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
			}

		private bool FeedbackExists (int id)
			{
			return _context.Feedback.Any(e => e.ID == id);
			}
		}
	}