using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineChronicle.Tables
{
    public class User
    {
        [Key] // Указывает, что это первичный ключ
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
        public int Id { get; set; }

        [Required] // Обязательное поле
        [Column(TypeName = "varchar(191)")]
        public string Email { get; set; }

        [Column(TypeName = "varchar(191)")]
        public string NickName { get; set; }

        [Required]
        [Column(TypeName = "varchar(191)")]
        public string Password { get; set; }

        [Column(TypeName = "varchar(191)")]
        public string NameIcon { get; set; }
    }
}