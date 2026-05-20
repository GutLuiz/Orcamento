using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Orcamento.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }

        public DateTime Date { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category ? Category { get; set; }
     
        [NotMapped]

        public string CategoryName { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User ? User { get; set; }
    }
}
