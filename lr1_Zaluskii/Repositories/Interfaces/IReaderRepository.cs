using lr1_Zaluskii.Models;

namespace lr1_Zaluskii.Repositories.Interfaces
{
    public interface IReaderRepository : IRepository<Reader>
    {
        Task<Reader?> GetByEmailAsync(string email);
        Task<Reader?> GetByLibraryCardAsync(string cardNumber);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<bool> LibraryCardExistsAsync(string cardNumber, int? excludeId = null);
    }
}
