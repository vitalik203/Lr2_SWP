using System.ComponentModel.DataAnnotations;

namespace lr1_Zaluskii.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва є обов'язковою")]
        [StringLength(100, ErrorMessage = "Назва не може перевищувати 100 символів")]
        [Display(Name = "Назва")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Автор є обов'язковим")]
        [StringLength(100, ErrorMessage = "Ім'я автора не може перевищувати 100 символів")]
        [Display(Name = "Автор")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN є обов'язковим")]
        [RegularExpression(@"^\d{3}-\d{1,5}-\d{1,7}-\d{1,7}-\d{1}$", ErrorMessage = "ISBN повинен бути у форматі 978-3-16-148410-0")]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; } = string.Empty;

        [Range(1000, 2100, ErrorMessage = "Введіть коректний рік")]
        [Display(Name = "Рік видання")]
        public int PublishedYear { get; set; }

        [Range(1, 10000, ErrorMessage = "Кількість сторінок повинна бути більше 0")]
        [Display(Name = "Кількість сторінок")]
        public int Pages { get; set; }

        [Display(Name = "Жанр")]
        public string Genre { get; set; } = string.Empty;

        public ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();
    }
}
