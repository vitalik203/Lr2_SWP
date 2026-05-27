using lr1_Zaluskii.Data;
using lr1_Zaluskii.Models;
using lr1_Zaluskii.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lr1_Zaluskii.Repositories
{
    public class BookIssueRepository : IBookIssueRepository
    {
        private readonly ApplicationDbContext _context;

        public BookIssueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookIssue>> GetAllAsync()
            => await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .ToListAsync();

        public async Task<BookIssue?> GetByIdAsync(int id)
            => await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .FirstOrDefaultAsync(bi => bi.Id == id);

        public async Task<BookIssue> CreateAsync(BookIssue entity)
        {
            _context.BookIssues.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<BookIssue?> UpdateAsync(int id, BookIssue entity)
        {
            var existing = await _context.BookIssues.FindAsync(id);
            if (existing is null) return null;

            existing.BookId = entity.BookId;
            existing.ReaderId = entity.ReaderId;
            existing.IssueDate = entity.IssueDate;
            existing.DueDate = entity.DueDate;
            existing.ReturnDate = entity.ReturnDate;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.BookIssues.FindAsync(id);
            if (entity is null) return false;

            _context.BookIssues.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookIssue>> GetByReaderAsync(int readerId)
            => await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .Where(bi => bi.ReaderId == readerId)
                .ToListAsync();

        public async Task<IEnumerable<BookIssue>> GetByBookAsync(int bookId)
            => await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .Where(bi => bi.BookId == bookId)
                .ToListAsync();

        public async Task<IEnumerable<BookIssue>> GetOverdueAsync()
            => await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .Where(bi => bi.ReturnDate == null && bi.DueDate < DateTime.Now)
                .ToListAsync();

        public async Task<IEnumerable<BookIssue>> GetActiveAsync()
            => await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Reader)
                .Where(bi => bi.ReturnDate == null)
                .ToListAsync();
    }
}
