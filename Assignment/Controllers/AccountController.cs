using Assignment.Data;
using Assignment.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace Assignment.Controllers
{
	public class AccountController : Controller
		{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ApplicationDbContext context)
			{
			_signInManager = signInManager;
			_userManager = userManager;
			_context = context;
			}

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
			// Create list of existing and available roles
			ViewBag.Name = new SelectList(_context.Roles.ToList(), "Name", "Name");
			return View();
			}

		[HttpPost]
		public async Task<IActionResult> Register (RegisterVM model)
			{
			if (ModelState.IsValid)
				{
				var user = new AppUser()
					{
					FirstName = model.FirstName,
					LastName = model.LastName,
					//TeamName = model.TeamName,
					Email = model.Email,
					UserName = model.Email
					};

				var result = await _userManager.CreateAsync(user, model.Password!);


				if (result.Succeeded)
					{
					await _userManager.AddToRoleAsync(user, model.Role);
					await _signInManager.SignInAsync(user, false);
					return RedirectToAction("Index", "Home");
					}
				foreach (var error in result.Errors) 
					{
					ModelState.AddModelError("", error.Description);
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

		// POST: Account/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditAccount (string id, [Bind("ID,Author,TargetUser,FeedbackDate,FeedbackText")] AppUser account)
			{
			if (id != account.Id)
				{
				return NotFound();
				}

			if (ModelState.IsValid)
				{
				try
					{
					_context.Update(account);
					await _context.SaveChangesAsync();
					}
				catch (DbUpdateConcurrencyException)
					{
					if (!AccountExists(account.Id))
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
