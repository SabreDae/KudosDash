using KudosDash.Data;
using KudosDash.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KudosDash.Controllers
	{
	public class AccountController (SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ApplicationDbContext context, ILogger<AccountController> logger) : Controller
		{
		private readonly SignInManager<AppUser> _signInManager = signInManager;
		private readonly UserManager<AppUser> _userManager = userManager;
		private readonly ApplicationDbContext _context = context;
		private readonly ILogger _logger = logger;

		[HttpGet]
		public IActionResult Login ()
			{
			return View();
			}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login (LoginVM model)
			{
			if (ModelState.IsValid)
				{
				var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password, model.RememberMe, true);

				if (result.Succeeded)
					{
					if (User.IsInRole("Admin"))
						{
						// If the user logging in has an admin account, redirect them to the Admin dashboard
						return RedirectToAction("Index", "Admin");
						}
					// Else, redirect the user to their own Feedback Dashboard
					return RedirectToAction("Index", "Feedback");
					}
				// Log all incorrect login attempts to provide an indication of brute force attempts
				_logger.LogWarning("User: {0}, invalid login attempt at {1}", model.Email, DateTime.UtcNow);
				ModelState.AddModelError("", "Login details were incorrect.");
				if (result.IsLockedOut)
					{
					return View("AccountLocked");
					}
				}
			return View(model);
			}

		[HttpGet]
		public IActionResult Register ()
			{
			// Create list of existing teams
			ViewBag.Team = new SelectList(_context.Teams.ToList(), "TeamId", "TeamName");

			// Create list of existing and available roles
			ViewBag.Role = new SelectList(_context.Roles.Where(r => r.Name != "Admin").ToList(), "Name", "Name");
			return View();
			}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register (RegisterVM model)
			{
			// Create list of existing teams
			ViewBag.Team = new SelectList(_context.Teams.ToList(), "TeamId", "TeamName");

			// Create list of existing and available roles
			ViewBag.Role = new SelectList(_context.Roles.Where(r => r.Name != "Admin").ToList(), "Name", "Name");

			if (ModelState.IsValid)
				{
				var user = new AppUser()
					{
					FirstName = model.FirstName,
					LastName = model.LastName,
					Team = model.TeamId,
					Email = model.Email,
					UserName = model.Email
					};

				var result = await _userManager.CreateAsync(user, model.Password!);

				if (result.Succeeded)
					{
					await _userManager.AddToRoleAsync(user, model.Role);
					await _signInManager.SignInAsync(user, false);
					if (model.Role == "Manager")
						{
						TempData["AlertMessage"] = "Don't forget to create your team!";
						}
					return RedirectToAction("Index", "Home");
					}
				foreach (var error in result.Errors)
					{
					ModelState.AddModelError("", error.Description);
					}
				}
			return View(model);
			}


		// GET: Account/Details/
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Details ()
			{
			// userId should always exist as unauthenticated users will be redirected to log into an account, all accounts have an Id
			var userId = _userManager.GetUserId(User);
			var user = await _context.Account
				.FirstOrDefaultAsync(m => m.Id == userId);

			if (user == null)
				{
				return View("Error");
				}

			// If a user has not previously joined a team, no team name will be found in the database
			int? teamName;
			if (user.TeamId == null)
				{
				// Set the team name instead to an empty string
				teamName = null;
				}
			else
				{
				// Where a team has been chosen previously, select this from the database to display in the view
				teamName = _context.Teams.Find(user.TeamId).TeamId;
				}

			ViewBag.TeamChoices = new SelectList(_context.Teams, "TeamId", "TeamName");

			var model = new AccountVM
				{
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				Username = user.Email,
				TeamName = teamName,
				};

			return View(model);
			}

		// POST: Account/Details/
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Details (AccountVM model)
			{
			var userId = _userManager.GetUserId(User);
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
				{
				return View("Error");
				}

			ViewBag.TeamChoices = new SelectList(_context.Teams, "TeamId", "TeamName");

			// Cache existing email address on account to verify changes against and revert to if needed
			var email = user.Email;

			// Get new details from view
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.Email = model.Email;
			user.UserName = model.Email;
			user.TeamId = _context.Teams.FirstOrDefault(t => t.TeamId == model.TeamName).TeamId;

			if (EmailExists(model.Email, email))
				{
				// Add error messages
				ModelState.AddModelError("", "Account details could not be updated. Please review error messages below and try again.");
				ModelState.AddModelError("Email", "Email unavailable");
				}

			if (ModelState.IsValid)
				{
				var result = await _userManager.UpdateAsync(user);
				if (result.Succeeded)
					{
					await _context.SaveChangesAsync();
					_logger.LogInformation("User updated {acc} at {DT}.", userId, DateTime.UtcNow);
					return View(model);
					}
				}

			return View(model);
			}

		private bool EmailExists (string requestedEmail, string originalEmail)
			{
			// Verify whether email change request made
			if (originalEmail == requestedEmail)
				{
				// If no request to change email address was made, return false
				return false;
				}
			// Else check for whether the new requested email exists already in the database and return true if found
			return _context.Account.Any(x => x.Email == requestedEmail);
			}

		private bool AccountExists (string id)
			{
			return _context.Account.Any(e => e.Id == id);
			}

		public async Task<IActionResult> Logout ()
			{
			await _signInManager.SignOutAsync();
			// Clear the user's cookie
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
			}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Delete ()
			{
			var id = _userManager.GetUserId(HttpContext.User);
			if (id == null)
				{
				return NotFound();
				}
			var account = await _context.Account.FindAsync(id);
			if (account == null)
				{
				return NotFound();
				}
			return View(account);
			}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed ()
			{
			var id = _userManager.GetUserId(HttpContext.User);
			//First Fetch the User you want to Delete
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
				{
				return NotFound();
				}
			else
				{
				//Delete the User Using DeleteAsync Method of UserManager Service
				var result = await _userManager.DeleteAsync(user);
				if (result.Succeeded)
					{
					// Handle a successful delete by ensuring no user is marked as logged in and redirecting to Home
					await _signInManager.SignOutAsync();
					_logger.LogInformation("User deleted account {Acc} at {DT}", id, DateTime.UtcNow);
					TempData["AlertMessage"] = "Account successfully deleted!";
					return RedirectToAction("Index", "Home");
					}
				return View("Delete");
				}
			}
		}
	}