using KudosDash.Data;
using KudosDash.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KudosDash.Controllers
	{
	public class TeamsController : Controller
		{
		private readonly ApplicationDbContext _context;

		public TeamsController (ApplicationDbContext context)
			{
			_context = context;
			}

		// GET: Teams
		// Only admins will have access to viewing and editing all teams, managers will be able to access information for their own team only
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Index ()
			{
			return View(await _context.Teams.ToListAsync());
			}

		// GET: Teams/Details/5
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Details (int? id)
			{
			if (id == null)
				{
				return NotFound();
				}

			var teams = await _context.Teams
				.FirstOrDefaultAsync(m => m.TeamId == id);
			if (teams == null)
				{
				return NotFound();
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
			if (ModelState.IsValid)
				{
				_context.Add(teams);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
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

			var teams = await _context.Teams.FindAsync(id);
			if (teams == null)
				{
				return NotFound();
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
					_context.Update(teams);
					await _context.SaveChangesAsync();
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
				return RedirectToAction(nameof(Index));
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

			var teams = await _context.Teams
				.FirstOrDefaultAsync(m => m.TeamId == id);
			if (teams == null)
				{
				return NotFound();
				}

			return View(teams);
			}

		// POST: Teams/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> DeleteConfirmed (int id)
			{
			var teams = await _context.Teams.FindAsync(id);
			if (teams != null)
				{
				_context.Teams.Remove(teams);
				}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
			}

		private bool TeamsExists (int id)
			{
			return _context.Teams.Any(e => e.TeamId == id);
			}
		}
	}