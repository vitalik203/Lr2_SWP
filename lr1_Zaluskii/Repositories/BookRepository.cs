using lr1_Zaluskii.Data;
using lr1_Zaluskii.Models;
using lr1_Zaluskii.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lr1_Zaluskii.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
            => await _context.Books.ToListAsync();

        public async Task<Book?> GetByIdAsync(int id)
            => await _context.Books.FindAsync(id);

        public async Task<Book> CreateAsync(Book entity)
        {
            _context.Books.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Book?> UpdateAsync(int id, Book entity)
        {
            var existing = await _context.Books.FindAsync(id);
            if (existing is null) return null;

            existing.Title = entity.Title;
            existing.Author = entity.Author;
            existing.ISBN = entity.ISBN;
            existing.PublishedYear = entity.PublishedYear;
            existing.Pages = entity.Pages;
            existing.Genre = entity.Genre;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Books.FindAsync(id);
            if (entity is null) return false;

            _context.Books.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Book>> GetByAuthorAsync(string author)
            => await _context.Books
                .Where(b => b.Author.Contains(author))
                .ToListAsync();

        public async Task<IEnumerable<Book>> GetByGenreAsync(string genre)
            => await _context.Books
                .Where(b => b.Genre.Contains(genre))
                .ToListAsync();

        public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null)
            => await _context.Books
                .AnyAsync(b => b.ISBN == isbn && (excludeId == null || b.Id != excludeId));
    }
}
