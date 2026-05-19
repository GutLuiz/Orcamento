using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Orcamento.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null;

        public int UserId { get; set; }
        public User ? User { get; set; }

        [JsonIgnore]
        public List<Transaction> Transactions { get; set; } = new();

        [NotMapped] // essa prop n existe no banco
        public decimal MovimentacaoMensal { get; set; } = 0;
    }
}
