using lr1_Zaluskii.Models;
using lr1_Zaluskii.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace lr1_Zaluskii.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookIssuesApiController : ControllerBase
    {
        private readonly IBookIssueRepository _repository;

        public BookIssuesApiController(IBookIssueRepository repository)
        {
            _repository = repository;
        }

        /// <summary>Отримати список усіх видач книг</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookIssue>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var issues = await _repository.GetAllAsync();
            return Ok(issues);
        }

        /// <summary>Отримати видачу за ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BookIssue), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var issue = await _repository.GetByIdAsync(id);
            return issue is null ? NotFound() : Ok(issue);
        }

        /// <summary>Отримати видачі за читачем</summary>
        [HttpGet("by-reader/{readerId:int}")]
        [ProducesResponseType(typeof(IEnumerable<BookIssue>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByReader(int readerId)
        {
            var issues = await _repository.GetByReaderAsync(readerId);
            return Ok(issues);
        }

        /// <summary>Отримати видачі за книгою</summary>
        [HttpGet("by-book/{bookId:int}")]
        [ProducesResponseType(typeof(IEnumerable<BookIssue>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByBook(int bookId)
        {
            var issues = await _repository.GetByBookAsync(bookId);
            return Ok(issues);
        }

        /// <summary>Отримати прострочені видачі</summary>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(IEnumerable<BookIssue>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOverdue()
        {
            var issues = await _repository.GetOverdueAsync();
            return Ok(issues);
        }

        /// <summary>Отримати активні видачі (не повернуті)</summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<BookIssue>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive()
        {
            var issues = await _repository.GetActiveAsync();
            return Ok(issues);
        }

        /// <summary>Створити нову видачу книги</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BookIssue), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] BookIssue issue)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _repository.CreateAsync(issue);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Оновити дані видачі книги</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(BookIssue), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] BookIssue issue)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _repository.UpdateAsync(id, issue);
            return updated is null ? NotFound() : Ok(updated);
        }

        /// <summary>Зареєструвати повернення книги</summary>
        [HttpPatch("{id:int}/return")]
        [ProducesResponseType(typeof(BookIssue), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Return(int id)
        {
            var issue = await _repository.GetByIdAsync(id);
            if (issue is null) return NotFound();
            if (issue.ReturnDate.HasValue) return BadRequest(new { message = "Книга вже була повернута." });

            issue.ReturnDate = DateTime.Now;
            var updated = await _repository.UpdateAsync(id, issue);
            return Ok(updated);
        }

        /// <summary>Видалити запис про видачу</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
