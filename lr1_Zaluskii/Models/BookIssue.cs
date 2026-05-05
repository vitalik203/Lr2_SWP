using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lr1_Zaluskii.Models
{
    public class BookIssue : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Книга є обов'язковою")]
        [Display(Name = "Книга")]
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [Required(ErrorMessage = "Читач є обов'язковим")]
        [Display(Name = "Читач")]
        public int ReaderId { get; set; }
        public Reader? Reader { get; set; }

        [Required]
        [Display(Name = "Дата видачі")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Термін повернення")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14);

        [Display(Name = "Дата повернення")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Повернуто")]
        public bool IsReturned => ReturnDate.HasValue;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DueDate <= IssueDate)
                yield return new ValidationResult(
                    "Термін повернення повинен бути пізніше дати видачі",
                    new[] { nameof(DueDate) });
        }
    }
}
