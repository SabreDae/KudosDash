﻿using KudosDash.Data;
using KudosDash.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KudosDash.Controllers
{
	// Ensure only logged in users are able to access Feedback views/model
	[Authorize]
	public class FeedbackController : Controller
	{
		private readonly ApplicationDbContext _context;

		public FeedbackController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Feedback Index page view
		public async Task<IActionResult> Index()
		{
			return View(await _context.Feedback.ToListAsync());
		}

		// GET: Feedback/Details/5
		public async Task<IActionResult> Details(int? id)
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
		public IActionResult Create()
		{
			return View();
		}

		// POST: Feedback/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ID,Author,TargetUser,FeedbackDate,FeedbackText")] Feedback feedback)
		{
			if (ModelState.IsValid)
			{
				_context.Add(feedback);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(feedback);
		}

		// GET: Feedback/Edit/5
		public async Task<IActionResult> Edit(int? id)
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
		public async Task<IActionResult> Edit(int id, [Bind("ID,Author,TargetUser,FeedbackDate,FeedbackText")] Feedback feedback)
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
		public async Task<IActionResult> Delete(int? id)
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var feedback = await _context.Feedback.FindAsync(id);
			if (feedback != null)
			{
				_context.Feedback.Remove(feedback);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool FeedbackExists(int id)
		{
			return _context.Feedback.Any(e => e.ID == id);
		}
	}
}