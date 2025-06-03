using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineChronicle.Tables
{
    public class ContentRecommendation
    {
        [Key] // Замена [PrimaryKey]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Замена AutoIncrement
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(191)")] // Оптимальный тип для MySQL
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Type { get; set; } // Например: "movie", "series"

        [Column(TypeName = "text")] // URL могут быть длинными
        public string ImageUrl { get; set; }

        [Required]
        [Column(TypeName = "datetime")] // Точный тип для даты/времени
        public DateTime DateChange { get; set; } = DateTime.UtcNow; // Значение по умолчанию
    }
}