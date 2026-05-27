using lr1_Zaluskii.Models;
using lr1_Zaluskii.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace lr1_Zaluskii.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BooksApiController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksApiController(IBookRepository repository)
        {
            _repository = repository;
        }

        /// <summary>Отримати список усіх книг</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var books = await _repository.GetAllAsync();
            return Ok(books);
        }

        /// <summary>Отримати книгу за ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        /// <summary>Отримати книги за автором</summary>
        [HttpGet("by-author")]
        [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAuthor([FromQuery] string author)
        {
            var books = await _repository.GetByAuthorAsync(author);
            return Ok(books);
        }

        /// <summary>Отримати книги за жанром</summary>
        [HttpGet("by-genre")]
        [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByGenre([FromQuery] string genre)
        {
            var books = await _repository.GetByGenreAsync(genre);
            return Ok(books);
        }

        /// <summary>Додати нову книгу</summary>
        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _repository.IsbnExistsAsync(book.ISBN))
                return Conflict(new { message = $"Книга з ISBN '{book.ISBN}' вже існує." });

            var created = await _repository.CreateAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Оновити книгу</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _repository.IsbnExistsAsync(book.ISBN, id))
                return Conflict(new { message = $"Книга з ISBN '{book.ISBN}' вже існує." });

            var updated = await _repository.UpdateAsync(id, book);
            return updated is null ? NotFound() : Ok(updated);
        }

        /// <summary>Видалити книгу</summary>
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
