using System.ComponentModel.DataAnnotations;

namespace WebBank.Models {
    public class ExpiredUserTokenDTOs {
        [Key]
        public int Id { get; set; } 
        public string Token { get; set; }
        public int UserId { get; set; }
    }
}
