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
	public class FeedbackController(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<FeedbackController> logger) : Controller
	{
		private readonly ILogger _logger = logger;

		[HttpGet]
		// GET: Feedback Index page view
		public async Task<IActionResult> Index()
		{
			var currentUser = userManager.GetUserId(User);
			if (currentUser == null)
			{
				return NotFound();
			}
			else
			{
				List<Feedback> feedbackRecords;
				if (User.IsInRole("Admin"))
				{
					// Generate a list of all Feedback entries
					feedbackRecords = await context.Feedback.ToListAsync();
				}
				else
				{
					if (User.IsInRole("Manager"))
					{
						/* Generate a list of Feedback entries where the value of target user in the User table 
						has the same teamId in the User table as the teamId of the Manager */
						var team = context.Account.Find(userManager.GetUserId(User)).TeamId;
						var teamMembers = await context.Account.Where(a => a.TeamId == team).ToListAsync();
						List<string> memberIds = [];
						foreach (AppUser member in teamMembers)
						{
							memberIds.Add(member.Id);
						}
						feedbackRecords = await context.Feedback.Where(f => memberIds.Contains(f.TargetUser)).ToListAsync();
					}
					else
					{
						// Generate a list of feedback entries where the target user is the current user and they have been approved by the team manager
						feedbackRecords = await context.Feedback.Where(f => f.TargetUser == currentUser && f.ManagerApproved == true).ToListAsync();
					}
					// Create a list of records that have been created by the logged in user - this populates a table to allow the user to edit feedback they submitted
					ViewBag.UserSubmittedFeedback = context.Feedback.Where(f => f.Author == currentUser).ToList();
				}
				var model = new FeedbackVM
				{
					feedback = feedbackRecords,
					user = await context.Account.ToListAsync()
				};
				return View(model);
			}
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Manager")]
		// GET: Feedback/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var requestedFeedback = context.Feedback.FirstOrDefault(m => m.Id == id);
			if (requestedFeedback == null)
			{
				return NotFound();
			}
			var feedback = new List<Feedback>
				{
				requestedFeedback
				};
			if (User.IsInRole("Manager"))
			{
				var user = await userManager.GetUserAsync(User);
				if (!FeedbackAuthorOrUserInManagersTeam(requestedFeedback, user))
				{
					_logger.LogWarning("User {U} made attempt to access unauthorized resource {F} at {DT}.", user.Email, id, DateTime.UtcNow);
					return RedirectToAction("AccessDenied");
				}
			}
			var model = new FeedbackVM
			{
				feedback = feedback,
				user = await context.Account.ToListAsync()
			};

			return View(model);
		}

		[HttpGet]
		// GET: Feedback/Create
		public async Task<IActionResult> Create()
		{
			var currentUser = await userManager.GetUserAsync(User);
			List<SelectListItem> selectListItems = [];
			Task<List<AppUser>> users;
			if (!User.IsInRole("Admin"))
			{
				if (currentUser.TeamId == null)
				{
					TempData["AlertMessage"] = "Please join a team before trying to submit feedback for colleagues!";
					return RedirectToAction("Details", "Account");
				}
				// Non-admin users should only be able to create feedback for users in the same team as themselves
				users = context.Account.Where(u => u.TeamId == currentUser.TeamId && u.Id != currentUser.Id).ToListAsync();
				if (users.Result.Count == 0)
				{
					// In some instances there will be no-one to submit feedback for so return error
					TempData["AlertMessage"] = "Sorry, it looks like your colleagues haven't joined this team yet.";
					return RedirectToAction("Index", "Home");
				}
			}
			else
			{
				// Populate list of all users
				users = context.Account.Where(u => u.Id != currentUser.Id).ToListAsync();
			}
			foreach (var user in await users)
			{
				selectListItems.Add(new SelectListItem() { Value = user.Id, Text = user.FirstName + " " + user.LastName });
			}
			ViewBag.Team = selectListItems;
			// Set date by default to current date
			Feedback model = new()
			{
				FeedbackDate = DateTime.Now,
			};
			return View(model);
		}

		// POST: Feedback/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ID,Author,TargetUser,FeedbackDate,FeedbackText")] Feedback feedback)
		{
			var currentUser = await userManager.GetUserAsync(User);
			if (!User.IsInRole("Admin"))
			{
				// Non-admin users should only be able to create feedback for users in the same team as themselves
				var users = context.Account.Where(u => u.TeamId == currentUser.TeamId && u.Id != currentUser.Id).ToListAsync();
				List<SelectListItem> selectListItems = [];
				foreach (var user in await users)
				{
					selectListItems.Add(new SelectListItem() { Value = user.Id, Text = user.FirstName + " " + user.LastName });
				}
				ViewBag.Team = selectListItems;
			}
			if (ModelState.IsValid)
			{
				feedback.Author = currentUser.Id;
				var result = context.Add(feedback);
				await context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(feedback);
		}

		[HttpGet]
		// GET: Feedback/Edit/5
		public async Task<IActionResult> Edit(int? id)
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

			var model = new EditFeedbackVM
			{
				Id = feedback.Id,
				TargetUser = feedback.TargetUser,
				FeedbackDate = feedback.FeedbackDate,
				FeedbackText = feedback.FeedbackText,
			};

			if (!User.IsInRole("Admin"))
			{
				var currentUser = await userManager.GetUserAsync(User);
				// Check whether the feedback Author and User are the same person
				var users = context.Account.Where(u => u.TeamId == currentUser.TeamId && u.Id != currentUser.Id).ToListAsync();
				List<SelectListItem> selectListItems = [];
				foreach (var user in await users)
				{
					selectListItems.Add(new SelectListItem() { Value = user.Id, Text = user.FirstName + " " + user.LastName });
				}
				ViewBag.TeamMembers = selectListItems;
				if (feedback.Author != currentUser.Id)
				{
					// non-admin users should only be able to edit self-authored records, so redirect them to the access denied view
					_logger.LogWarning("User {U} made attempt to access unauthorized resource {F} at {DT}.", currentUser.Email, id, DateTime.UtcNow);
					return RedirectToAction("AccessDenied");
				}
			}

			return View(model);
		}

		// POST: Feedback/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, EditFeedbackVM model)
		{
			var feedback = await context.Feedback.FirstOrDefaultAsync(f => f.Id == id);
			if (feedback == null)
			{
				return NotFound();
			}

			// Get new details from view
			feedback.FeedbackDate = model.FeedbackDate;
			feedback.FeedbackText = model.FeedbackText;
			feedback.ManagerApproved = false;

			if (ModelState.IsValid)
			{
				context.Update(feedback);
				await context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(feedback);
		}

		// GET: Feedback/Delete/5
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var feedback = await context.Feedback
				.FirstOrDefaultAsync(m => m.Id == id);
			if (feedback == null)
			{
				return NotFound();
			}
			if (User.IsInRole("Manager"))
			{
				var user = await userManager.GetUserAsync(User);
				if (!FeedbackAuthorOrUserInManagersTeam(feedback, user))
				{
					_logger.LogWarning("User {U} made attempt to delete unauthorized resource {F} at {DT}.", user.Email, feedback.Id, DateTime.UtcNow);
					return RedirectToAction("AccessDenied");
				}
			}

			return View(feedback);
		}

		// POST: Feedback/Delete/5
		[HttpPost, ActionName("Delete")]
		[Authorize(Roles = "Admin,Manager")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var feedback = await context.Feedback.FindAsync(id);
			if (feedback != null)
			{
				context.Feedback.Remove(feedback);
				_logger.LogInformation("Feedback record {F} deleted at {DT} by {u}", id, DateTime.UtcNow, User);
			}

			await context.SaveChangesAsync();
			TempData["AlertMessage"] = "Feedback deleted successfully.";
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		[Authorize(Roles = "Manager")]
		public async Task<IActionResult> ManagerApprove(int id)
		{
			var feedback = await context.Feedback.FindAsync(id);
			if (feedback == null)
			{
				return NotFound();
			}
			var model = new FeedbackVM
			{
				feedback = new List<Feedback>
				{
				feedback
				},
				user = await context.Account.ToListAsync()
			};

			var user = await userManager.GetUserAsync(User);
			if (!FeedbackAuthorOrUserInManagersTeam(feedback, user))
			{
				_logger.LogWarning("User {U} made attempt to access unauthorized resource {F} at {DT}.", user.Email, feedback.Id, DateTime.UtcNow);
				return RedirectToAction("AccessDenied");
			}

			return View(model);
		}

		[HttpPost, ActionName("ManagerApprove")]
		[Authorize(Roles = "Manager")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ManagerApproved(int id)
		{
			var feedback = await context.Feedback.FindAsync(id);

			if (feedback == null)
			{
				return NotFound();
			}

			feedback.ManagerApproved = true;
			try
			{
				var user = await userManager.GetUserAsync(User);
				context.Feedback.Update(feedback);
				_logger.LogInformation("Feedback record {F} made visible to target user at {DT} by {U}", id, DateTime.UtcNow, user.Email);
				await context.SaveChangesAsync();
				TempData["AlertMessage"] = string.Format("Feedback approved for {0} to view.", context.Account.Where(u => u.Id == feedback.TargetUser).FirstOrDefault().FirstName);
			}

			catch
			{
				TempData["AlertMessage"] = "Feedback could not be approved. Please try again.";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		[Authorize]
		public IActionResult AccessDenied()
		{
			return View();
		}

		private bool FeedbackAuthorOrUserInManagersTeam(Feedback feedback, AppUser user)
		{
			var teamMembers = context.Account.Where(a => a.TeamId == user.TeamId).ToList();
			var author = context.Account.Find(feedback.Author);
			var targetUser = context.Account.Find(feedback.TargetUser);
			return teamMembers.Contains(author) || teamMembers.Contains(targetUser);
		}
	}
}