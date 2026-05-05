using System.ComponentModel.DataAnnotations;

namespace lr1_Zaluskii.Models
{
    public class Reader
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        [StringLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Прізвище є обов'язковим")]
        [StringLength(50, ErrorMessage = "Прізвище не може перевищувати 50 символів")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [StringLength(100, ErrorMessage = "Email не може перевищувати 100 символів")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Невірний формат телефону")]
        [StringLength(20, ErrorMessage = "Телефон не може перевищувати 20 символів")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Номер квитка є обов'язковим")]
        [StringLength(20, ErrorMessage = "Номер квитка не може перевищувати 20 символів")]
        [Display(Name = "Номер читацького квитка")]
        public string LibraryCardNumber { get; set; } = string.Empty;

        [Display(Name = "Дата реєстрації")]
        [DataType(DataType.Date)]
        public DateTime DateRegistered { get; set; } = DateTime.Now;

        [Display(Name = "Повне ім'я")]
        public string FullName => $"{LastName} {FirstName}";

        public ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();
    }
}
