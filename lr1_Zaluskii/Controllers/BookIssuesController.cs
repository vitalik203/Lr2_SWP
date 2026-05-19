using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lr1_Zaluskii.Data;
using lr1_Zaluskii.Models;
using System.Text.Json;

namespace lr1_Zaluskii.Controllers
{
    public class BookIssuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookIssuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BookIssues
        public async Task<IActionResult> Index()
        {
            var issues = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .ToListAsync();
            return View(issues);
        }

        // GET: BookIssues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bookIssue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bookIssue == null) return NotFound();

            return View(bookIssue);
        }

        // GET: BookIssues/Create
        [Authorize]
        public IActionResult Create()
        {
            var json = HttpContext.Session.GetString("CreateBookIssue");
            BookIssue bookIssue = new BookIssue();
            if (json != null)
                bookIssue = JsonSerializer.Deserialize<BookIssue>(json) ?? new BookIssue();

            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bookIssue.BookId);
            ViewData["ReaderId"] = new SelectList(_context.Readers.Select(r => new { r.Id, FullName = r.LastName + " " + r.FirstName }), "Id", "FullName", bookIssue.ReaderId);
            return View(bookIssue);
        }

        // POST: BookIssues/Create — зберігає до сесії
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("BookId,ReaderId,IssueDate,DueDate")] BookIssue bookIssue)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("CreateBookIssue", JsonSerializer.Serialize(bookIssue));
                TempData["SessionInfo"] = "Дані збережено в сесії. Натисніть «Записати до бази» для фінального збереження.";
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bookIssue.BookId);
            ViewData["ReaderId"] = new SelectList(_context.Readers.Select(r => new { r.Id, FullName = r.LastName + " " + r.FirstName }), "Id", "FullName", bookIssue.ReaderId);
            return View(bookIssue);
        }

        // POST: BookIssues/SaveCreate — з сесії до бази, очистка сесії
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCreate()
        {
            var json = HttpContext.Session.GetString("CreateBookIssue");
            if (json == null)
            {
                TempData["Error"] = "Сесія порожня. Спочатку заповніть та збережіть форму.";
                return RedirectToAction(nameof(Create));
            }
            var bookIssue = JsonSerializer.Deserialize<BookIssue>(json)!;
            _context.Add(bookIssue);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("CreateBookIssue");
            return RedirectToAction(nameof(Index));
        }

        // GET: BookIssues/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bookIssue = await _context.BookIssues.FindAsync(id);
            if (bookIssue == null) return NotFound();

            HttpContext.Session.SetString("EditBookIssue", JsonSerializer.Serialize(bookIssue));
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bookIssue.BookId);
            ViewData["ReaderId"] = new SelectList(_context.Readers.Select(r => new { r.Id, FullName = r.LastName + " " + r.FirstName }), "Id", "FullName", bookIssue.ReaderId);
            return View(bookIssue);
        }

        // POST: BookIssues/Edit/5 — оновлює сесію (не БД)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,BookId,ReaderId,IssueDate,DueDate,ReturnDate")] BookIssue bookIssue)
        {
            if (id != bookIssue.Id) return NotFound();

            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("EditBookIssue", JsonSerializer.Serialize(bookIssue));
                TempData["SessionInfo"] = "Зміни збережено в сесії. Натисніть «Записати до бази» для фінального збереження.";
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bookIssue.BookId);
            ViewData["ReaderId"] = new SelectList(_context.Readers.Select(r => new { r.Id, FullName = r.LastName + " " + r.FirstName }), "Id", "FullName", bookIssue.ReaderId);
            return View(bookIssue);
        }

        // POST: BookIssues/SaveEdit — з сесії до бази, очистка сесії
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit()
        {
            var json = HttpContext.Session.GetString("EditBookIssue");
            if (json == null)
            {
                TempData["Error"] = "Сесія порожня. Відкрийте форму редагування та збережіть зміни.";
                return RedirectToAction(nameof(Index));
            }
            var bookIssue = JsonSerializer.Deserialize<BookIssue>(json)!;
            try
            {
                _context.Update(bookIssue);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("EditBookIssue");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BookIssues.Any(e => e.Id == bookIssue.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: BookIssues/Return/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            var bookIssue = await _context.BookIssues.FindAsync(id);
            if (bookIssue == null) return NotFound();

            bookIssue.ReturnDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: BookIssues/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bookIssue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bookIssue == null) return NotFound();

            return View(bookIssue);
        }

        // POST: BookIssues/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookIssue = await _context.BookIssues.FindAsync(id);
            if (bookIssue != null)
            {
                _context.BookIssues.Remove(bookIssue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
