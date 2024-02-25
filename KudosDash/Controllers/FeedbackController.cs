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
	public class FeedbackController (ApplicationDbContext context, UserManager<AppUser> userManager) : Controller
		{

		[HttpGet]
		// GET: Feedback Index page view
		public async Task<IActionResult> Index ()
			{
			var currentUser = userManager.GetUserId(User);
			if (currentUser == null)
				{
				return NotFound();
				}
			else
				{
				if (User.IsInRole("Admin"))
					{
					// Return all Feedback entries
					return View(await context.Feedback.ToListAsync());
					}
				else
					{
					if (User.IsInRole("Manager"))
						{
						/* Return a list of Feedback entries where the value of target user in the User table 
						has the same teamId in the User table as the teamId of the Manager */
						var team = context.Account.Find(userManager.GetUserId(User)).TeamId;
						var teamMembers = await context.Account.Where(a => a.TeamId == team).ToListAsync();
						List<string> memberIds = [];
						foreach (AppUser member in teamMembers)
							{
							memberIds.Add(member.Id);
							}
						return View(await context.Feedback.Where(f => memberIds.Contains(f.TargetUser)).ToListAsync());
						}
					// Return only feedback entries where the target user is the current user and they have been approved by the team manager
					return View(await context.Feedback.Where(f => f.TargetUser == currentUser && f.ManagerApproved == true).ToListAsync());
					}
				}
			}

		[HttpGet]
		[Authorize(Roles = "Admin,Manager")]
		// GET: Feedback/Details/5
		public async Task<IActionResult> Details (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var feedback = await context.Feedback
				.FirstOrDefaultAsync(m => m.ID == id);
			if (feedback == null)
				{
				return NotFound();
				}

			return View(feedback);
			}

		[HttpGet]
		// GET: Feedback/Create
		public async Task<IActionResult> Create ()
			{
			var currentUser = await userManager.GetUserAsync(User);
			if (!User.IsInRole("Admin"))
				{
				// Non-admin users should only be able to create feedback for users in the same team as themselves
				var users = context.Account.Where(u => u.TeamId == currentUser.TeamId).ToListAsync();
				List<SelectListItem> selectListItems = [];
				foreach (var user in await users)
					{
					// Ensure user will not be able to select themselves in Target User dropdown
					if (user.Id != currentUser.Id)
						{
						selectListItems.Add(new SelectListItem() { Value = user.Id, Text = user.FirstName + " " + user.LastName });
						}
					}
				ViewBag.Team = selectListItems;
				}
			// Set date by default to current date
			Feedback model = new()
				{
				FeedbackDate = DateTime.Now,
				Author = currentUser.Id
				};
			return View(model);
			}

		// POST: Feedback/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create ([Bind("ID,Author,TargetUser,FeedbackDate,FeedbackText")] Feedback feedback)
			{
			var currentUser = await userManager.GetUserAsync(User);
			if (!User.IsInRole("Admin"))
				{
				// Non-admin users should only be able to create feedback for users in the same team as themselves
				var users = context.Account.Where(u => u.TeamId == currentUser.TeamId).ToListAsync();
				List<SelectListItem> selectListItems = [];
				foreach (var user in await users)
					{
					// Ensure user will not be able to select themselves in Target User dropdown
					if (user.Id != currentUser.Id)
						{
						selectListItems.Add(new SelectListItem() { Value = user.Id, Text = user.FirstName + " " + user.LastName });
						}
					}
				ViewBag.Team = selectListItems;
				}
			if (ModelState.IsValid)
				{
				context.Add(feedback);
				await context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
				}
			return View(feedback);
			}

		[HttpGet]
		// GET: Feedback/Edit/5
		public async Task<IActionResult> Edit (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var feedback = await context.Feedback.FindAsync(id);
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
					context.Update(feedback);
					await context.SaveChangesAsync();
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
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Delete (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var feedback = await context.Feedback
				.FirstOrDefaultAsync(m => m.ID == id);
			if (feedback == null)
				{
				return NotFound();
				}

			return View(feedback);
			}

		// POST: Feedback/Delete/5
		[HttpPost, ActionName("Delete")]
		[Authorize(Roles = "Admin,Manager")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed (int id)
			{
			var feedback = await context.Feedback.FindAsync(id);
			if (feedback != null)
				{
				context.Feedback.Remove(feedback);
				}

			await context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
			}

		private bool FeedbackExists (int id)
			{
			return context.Feedback.Any(e => e.ID == id);
			}
		}
	}