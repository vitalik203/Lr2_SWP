using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lr1_Zaluskii.Data;
using lr1_Zaluskii.Models;
using System.Text.Json;

namespace lr1_Zaluskii.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.BookIssues)
                .ThenInclude(bi => bi.Reader)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            var json = HttpContext.Session.GetString("CreateBook");
            var book = json != null
                ? JsonSerializer.Deserialize<Book>(json) ?? new Book()
                : new Book();
            return View(book);
        }

        // POST: Books/Create — зберігає дані у сесію
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Author,ISBN,PublishedYear,Pages,Genre")] Book book)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("CreateBook", JsonSerializer.Serialize(book));
                TempData["SessionInfo"] = "Дані збережено в сесії. Натисніть «Записати до бази» для фінального збереження.";
            }
            return View(book);
        }

        // POST: Books/SaveCreate — записує з сесії до бази і очищує сесію
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCreate()
        {
            var json = HttpContext.Session.GetString("CreateBook");
            if (json == null)
            {
                TempData["Error"] = "Сесія порожня. Спочатку заповніть та збережіть форму.";
                return RedirectToAction(nameof(Create));
            }
            var book = JsonSerializer.Deserialize<Book>(json)!;
            _context.Add(book);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("CreateBook");
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            // Завантажуємо оригінальні дані до сесії
            HttpContext.Session.SetString("EditBook", JsonSerializer.Serialize(book));
            return View(book);
        }

        // POST: Books/Edit/5 — оновлює дані у сесії (не в БД)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Title,Author,ISBN,PublishedYear,Pages,Genre")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("EditBook", JsonSerializer.Serialize(book));
                TempData["SessionInfo"] = "Зміни збережено в сесії. Натисніть «Записати до бази» для фінального збереження.";
            }
            return View(book);
        }

        // POST: Books/SaveEdit — записує з сесії до бази і очищує сесію
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit()
        {
            var json = HttpContext.Session.GetString("EditBook");
            if (json == null)
            {
                TempData["Error"] = "Сесія порожня. Відкрийте форму редагування та збережіть зміни.";
                return RedirectToAction(nameof(Index));
            }
            var book = JsonSerializer.Deserialize<Book>(json)!;
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("EditBook");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.Id == book.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
