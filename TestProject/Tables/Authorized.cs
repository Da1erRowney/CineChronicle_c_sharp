using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineChronicle.Tables
{
    public class Authorized
    {
        [Key] // Указывает, что это первичный ключ
        [Column(TypeName = "varchar(191)")] // Оптимальный тип для MySQL
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}