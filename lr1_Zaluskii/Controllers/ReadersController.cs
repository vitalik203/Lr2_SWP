using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lr1_Zaluskii.Data;
using lr1_Zaluskii.Models;
using System.Text.Json;

namespace lr1_Zaluskii.Controllers
{
    public class ReadersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReadersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Readers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Readers.ToListAsync());
        }

        // GET: Readers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reader = await _context.Readers
                .Include(r => r.BookIssues)
                .ThenInclude(bi => bi.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reader == null) return NotFound();

            return View(reader);
        }

        // GET: Readers/Create
        [Authorize]
        public IActionResult Create()
        {
            var json = HttpContext.Session.GetString("CreateReader");
            var reader = json != null
                ? JsonSerializer.Deserialize<Reader>(json) ?? new Reader()
                : new Reader();
            return View(reader);
        }

        // POST: Readers/Create — зберігає до сесії
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstName,LastName,Email,Phone,LibraryCardNumber,DateRegistered")] Reader reader)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("CreateReader", JsonSerializer.Serialize(reader));
                TempData["SessionInfo"] = "Дані збережено в сесії. Натисніть «Записати до бази» для фінального збереження.";
            }
            return View(reader);
        }

        // POST: Readers/SaveCreate — з сесії до бази, очистка сесії
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCreate()
        {
            var json = HttpContext.Session.GetString("CreateReader");
            if (json == null)
            {
                TempData["Error"] = "Сесія порожня. Спочатку заповніть та збережіть форму.";
                return RedirectToAction(nameof(Create));
            }
            var reader = JsonSerializer.Deserialize<Reader>(json)!;
            _context.Add(reader);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("CreateReader");
            return RedirectToAction(nameof(Index));
        }

        // GET: Readers/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reader = await _context.Readers.FindAsync(id);
            if (reader == null) return NotFound();

            HttpContext.Session.SetString("EditReader", JsonSerializer.Serialize(reader));
            return View(reader);
        }

        // POST: Readers/Edit/5 — оновлює сесію (не БД)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone,LibraryCardNumber,DateRegistered")] Reader reader)
        {
            if (id != reader.Id) return NotFound();

            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("EditReader", JsonSerializer.Serialize(reader));
                TempData["SessionInfo"] = "Зміни збережено в сесії. Натисніть «Записати до бази» для фінального збереження.";
            }
            return View(reader);
        }

        // POST: Readers/SaveEdit — з сесії до бази, очистка сесії
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit()
        {
            var json = HttpContext.Session.GetString("EditReader");
            if (json == null)
            {
                TempData["Error"] = "Сесія порожня. Відкрийте форму редагування та збережіть зміни.";
                return RedirectToAction(nameof(Index));
            }
            var reader = JsonSerializer.Deserialize<Reader>(json)!;
            try
            {
                _context.Update(reader);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("EditReader");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Readers.Any(e => e.Id == reader.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Readers/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reader = await _context.Readers.FirstOrDefaultAsync(m => m.Id == id);
            if (reader == null) return NotFound();

            return View(reader);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader != null)
            {
                _context.Readers.Remove(reader);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
