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
	public class AccountController (SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ApplicationDbContext context) : Controller
		{
		private readonly SignInManager<AppUser> _signInManager = signInManager;
		private readonly UserManager<AppUser> _userManager = userManager;
		private readonly ApplicationDbContext _context = context;

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
				var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password, model.RememberMe, false);

				if (result.Succeeded)
					{
					return RedirectToAction("Index", "Home");
					}
				ModelState.AddModelError("", "Login details were incorrect.");
				}
			return View(model);
			}

		[HttpGet]
		public IActionResult Register ()
			{
			// Create list of existing teams
			ViewBag.Team = new SelectList(_context.Teams.ToList(), "TeamId", "TeamName");

			// Create list of existing and available roles
			ViewBag.Role = new SelectList(_context.Roles.ToList(), "Name", "Name");
			return View();
			}

		[HttpPost]
		public async Task<IActionResult> Register (RegisterVM model)
			{
			// Create list of existing teams
			ViewBag.Team = new SelectList(_context.Teams.ToList(), "TeamId", "TeamName");

			// Create list of existing and available roles
			ViewBag.Role = new SelectList(_context.Roles.ToList(), "Name", "Name");

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
				Console.WriteLine(result);

				if (result.Succeeded)
					{
					await _userManager.AddToRoleAsync(user, model.Role);
					await _signInManager.SignInAsync(user, false);
					return RedirectToAction("Index", "Home");
					}
				foreach (var error in result.Errors)
					{
					ModelState.AddModelError("", error.Description);
					Console.WriteLine(error.Description);
					}
				}
			return View(model);
			}


		// GET: Account/Details/5
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Details ()
			{
			var id = _userManager.GetUserId(HttpContext.User);
			if (id == null)
				{
				return View("Error");
				}

			var account = await _context.Account
				.FirstOrDefaultAsync(m => m.Id == id);
			if (account == null)
				{
				return View("Error");
				}

			return View(account);
			}

		// GET: Account/Edit/5
		[HttpGet]
		public async Task<IActionResult> Edit ()
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
		}
	}