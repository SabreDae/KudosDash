using KudosDash.Data;
using KudosDash.Models;
using KudosDash.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KudosDash.Controllers
	{
	[Authorize(Roles = "Admin,Manager")]
	public class TeamsController (ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<TeamsController> logger) : Controller
		{
		private readonly ILogger _logger = logger;

		// GET: Teams
		// Only admins will have access to viewing and editing all teams, managers will be able to access information for their own team only
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Index ()
			{
			return View(await context.Teams.ToListAsync());
			}

		// GET: Teams/Details/5
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Details (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var teams = await context.Teams
				.FirstOrDefaultAsync(m => m.TeamId == id);
			if (teams == null)
				{
				return NotFound();
				}

			if (User.IsInRole("Manager"))
				{
				var user = await userManager.GetUserAsync(User);
				if (user.TeamId == null)
					{
						// If the manager hasn't registered a team, return them to the Home Page
						TempData["AlertMessage"] = "You don't seem to have created a team yet."
						return RedirectToAction("Create");
					}
				if (!IsTeamManager((int)id, user))
					{
					// Redirect to the correct Team view
					return RedirectToAction("Details", new { id = user.TeamId });
					}
				var userId = userManager.GetUserId(User);
				// Create list of users in the same team as the manager, with the manager themself excluded
				List<AppUser> membersList = await context.Account.Where(a => a.TeamId == id && a.Id != userId).ToListAsync();
				ViewBag.Members = membersList;
				}

			return View(teams);
			}

		// GET: Teams/Create
		[Authorize(Roles = "Admin,Manager")]
		public IActionResult Create ()
			{
			return View();
			}

		// POST: Teams/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[Authorize(Roles = "Admin,Manager")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create ([Bind("TeamId,TeamName")] Teams teams)
			{
			if (TeamNameExists(teams.TeamName))
				{
				// Add error messages
				ModelState.AddModelError("", "Team could not be created. Please review any errors below and try again.");
				ModelState.AddModelError("TeamName", "A team with this name already exists.");
				}

			if (ModelState.IsValid)
				{
				context.Add(teams);
				// Save new team in teams table
				await context.SaveChangesAsync();

				if (User.IsInRole("Manager"))
					{
					/* When a manager creates a team, their user record needs to be updated to be associated to that team.
					This is because the manager should only have visibility of feedback entries associated to users 
					in the same team as the one they manage. Admin users in contrast should not be associated to any 
					team as they need to be able to see ALL records.
					 */
					var userId = userManager.GetUserId(HttpContext.User);
					var user = await context.Account
						.FirstOrDefaultAsync(m => m.Id == userId);

					user.TeamId = teams.TeamId;
					// Save user changes
					await context.SaveChangesAsync();
					}
				return RedirectToAction("Details", new { id = teams.TeamId });
				}
			return View(teams);
			}

		// GET: Teams/Edit/5
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Edit (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var teams = await context.Teams.FindAsync(id);
			if (teams == null)
				{
				return NotFound();
				}

			if (User.IsInRole("Manager"))
				{
				var user = await userManager.GetUserAsync(User);
				if (user.TeamId == null)
					{
						// If the manager hasn't registered a team, return them to the Home Page
						TempData["AlertMessage"] = "You don't seem to have created a team yet."
						return RedirectToAction("Create");
					}
				if (!IsTeamManager((int)id, user))
					{
					// Redirect to the correct Team view
					return RedirectToAction("Edit", new { id = user.TeamId });
					}
				}

			return View(teams);
			}

		// POST: Teams/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Edit (int id, [Bind("TeamId,TeamName")] Teams teams)
			{
			if (id != teams.TeamId)
				{
				return NotFound();
				}

			if (ModelState.IsValid)
				{
				try
					{
					context.Update(teams);
					await context.SaveChangesAsync();
					}
				catch (DbUpdateConcurrencyException)
					{
					if (!TeamsExists(teams.TeamId))
						{
						return NotFound();
						}
					else
						{
						throw;
						}
					}
				return RedirectToAction("Details", new { id = teams.TeamId });
				}
			return View(teams);
			}

		// GET: Teams/Delete/5
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Delete (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var teams = await context.Teams
				.FirstOrDefaultAsync(m => m.TeamId == id);
			if (teams == null)
				{
				return NotFound();
				}

			if (User.IsInRole("Manager"))
				{
				var user = await userManager.GetUserAsync(User);
				if (!IsTeamManager((int)id, user))
					{
						try 
						{
							// Redirect to the correct Team view if the manager has a team
							return RedirectToAction("Delete", new { id = user.TeamId });
						} catch
						{
							// Else simply redirect to Home
							TempData["AlertMessage"] = "Sorry, we couldn't find your team! Are you sure you've created one?";
							return RedirectToAction("Index", "Home");
						}
					}
				}

			return View(teams);
			}

		// POST: Teams/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> DeleteConfirmed (int id)
			{
			var teams = await context.Teams.FindAsync(id);
			if (teams != null)
				{
				Console.WriteLine("Attempting delete");
				context.Teams.Remove(teams);
				// Ensure references to the deleted team are removed from other tables
				// await context.SaveChangesAsync();
				TempData["AlertMessage"] = "Team has successfully been deleted!";
				_logger.LogInformation("Team record {T} deleted at {DT} by {u}", id, DateTime.UtcNow, User);
				}
			if (User.IsInRole("Manager"))
				{
				// Manager will be redirected to home page on successful delete as they have no access to the Teams Index
				return RedirectToAction("Index", "Home");
				}
			else
				{
				return RedirectToAction("Index");
				}
			}

		private bool TeamsExists (int id)
			{
			return context.Teams.Any(e => e.TeamId == id);
			}

		private bool TeamNameExists (string teamName)
			{
			return context.Teams.Any(m => m.TeamName == teamName);
			}

		private bool IsTeamManager (int id, AppUser user)
			{
			// Return boolean status for whether the user is the Manager of the requested team ID
			return user.TeamId == id;
			}
		}
	}