using lr1_Zaluskii.Models;

namespace lr1_Zaluskii.Repositories.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetByAuthorAsync(string author);
        Task<IEnumerable<Book>> GetByGenreAsync(string genre);
        Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null);
    }
}
