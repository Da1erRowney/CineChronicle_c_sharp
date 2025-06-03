using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineChronicle.Tables
{
    public class DateExit
    {
        [Key] // Заменяем [PrimaryKey] на [Key]
        [Column(TypeName = "varchar(191)")] // Оптимальный тип для MySQL
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "varchar(191)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")] // Формат даты как строки
        public string DateRelease { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string SendStatus { get; set; }
    }
}