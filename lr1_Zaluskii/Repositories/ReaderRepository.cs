using lr1_Zaluskii.Data;
using lr1_Zaluskii.Models;
using lr1_Zaluskii.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lr1_Zaluskii.Repositories
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly ApplicationDbContext _context;

        public ReaderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reader>> GetAllAsync()
            => await _context.Readers.ToListAsync();

        public async Task<Reader?> GetByIdAsync(int id)
            => await _context.Readers.FindAsync(id);

        public async Task<Reader> CreateAsync(Reader entity)
        {
            _context.Readers.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Reader?> UpdateAsync(int id, Reader entity)
        {
            var existing = await _context.Readers.FindAsync(id);
            if (existing is null) return null;

            existing.FirstName = entity.FirstName;
            existing.LastName = entity.LastName;
            existing.Email = entity.Email;
            existing.Phone = entity.Phone;
            existing.LibraryCardNumber = entity.LibraryCardNumber;
            existing.DateRegistered = entity.DateRegistered;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Readers.FindAsync(id);
            if (entity is null) return false;

            _context.Readers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Reader?> GetByEmailAsync(string email)
            => await _context.Readers.FirstOrDefaultAsync(r => r.Email == email);

        public async Task<Reader?> GetByLibraryCardAsync(string cardNumber)
            => await _context.Readers.FirstOrDefaultAsync(r => r.LibraryCardNumber == cardNumber);

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
            => await _context.Readers
                .AnyAsync(r => r.Email == email && (excludeId == null || r.Id != excludeId));

        public async Task<bool> LibraryCardExistsAsync(string cardNumber, int? excludeId = null)
            => await _context.Readers
                .AnyAsync(r => r.LibraryCardNumber == cardNumber && (excludeId == null || r.Id != excludeId));
    }
}
