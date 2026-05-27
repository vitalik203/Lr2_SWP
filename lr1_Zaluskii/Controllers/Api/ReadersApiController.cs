using lr1_Zaluskii.Models;
using lr1_Zaluskii.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace lr1_Zaluskii.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReadersApiController : ControllerBase
    {
        private readonly IReaderRepository _repository;

        public ReadersApiController(IReaderRepository repository)
        {
            _repository = repository;
        }

        /// <summary>Отримати список усіх читачів</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Reader>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var readers = await _repository.GetAllAsync();
            return Ok(readers);
        }

        /// <summary>Отримати читача за ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Reader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var reader = await _repository.GetByIdAsync(id);
            return reader is null ? NotFound() : Ok(reader);
        }

        /// <summary>Отримати читача за email</summary>
        [HttpGet("by-email")]
        [ProducesResponseType(typeof(Reader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var reader = await _repository.GetByEmailAsync(email);
            return reader is null ? NotFound() : Ok(reader);
        }

        /// <summary>Отримати читача за номером квитка</summary>
        [HttpGet("by-card")]
        [ProducesResponseType(typeof(Reader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByLibraryCard([FromQuery] string cardNumber)
        {
            var reader = await _repository.GetByLibraryCardAsync(cardNumber);
            return reader is null ? NotFound() : Ok(reader);
        }

        /// <summary>Додати нового читача</summary>
        [HttpPost]
        [ProducesResponseType(typeof(Reader), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] Reader reader)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _repository.EmailExistsAsync(reader.Email))
                return Conflict(new { message = $"Читач з email '{reader.Email}' вже існує." });

            if (await _repository.LibraryCardExistsAsync(reader.LibraryCardNumber))
                return Conflict(new { message = $"Читач з номером квитка '{reader.LibraryCardNumber}' вже існує." });

            var created = await _repository.CreateAsync(reader);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Оновити дані читача</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Reader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, [FromBody] Reader reader)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _repository.EmailExistsAsync(reader.Email, id))
                return Conflict(new { message = $"Читач з email '{reader.Email}' вже існує." });

            if (await _repository.LibraryCardExistsAsync(reader.LibraryCardNumber, id))
                return Conflict(new { message = $"Читач з номером квитка '{reader.LibraryCardNumber}' вже існує." });

            var updated = await _repository.UpdateAsync(id, reader);
            return updated is null ? NotFound() : Ok(updated);
        }

        /// <summary>Видалити читача</summary>
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
