using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineChronicle.Tables
{
    public class UserSettings
    {
        [Key] // Первичный ключ
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
        public int UserId { get; set; }

        [Column(TypeName = "tinyint(1)")] // Оптимальный тип для bool в MySQL
        public bool IsDarkTheme { get; set; }

        [Column(TypeName = "tinyint(1)")]
        public bool IsVideoBackground { get; set; }
    }
}