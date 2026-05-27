using lr1_Zaluskii.Models;

namespace lr1_Zaluskii.Repositories.Interfaces
{
    public interface IBookIssueRepository : IRepository<BookIssue>
    {
        Task<IEnumerable<BookIssue>> GetByReaderAsync(int readerId);
        Task<IEnumerable<BookIssue>> GetByBookAsync(int bookId);
        Task<IEnumerable<BookIssue>> GetOverdueAsync();
        Task<IEnumerable<BookIssue>> GetActiveAsync();
    }
}
