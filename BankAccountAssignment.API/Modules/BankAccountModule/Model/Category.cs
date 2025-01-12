using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankAccountAssignment.API.Modules.BankAccountModule.Model
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }
    }
    public enum CategoryTypeEnum {
        Savings = 1,
        Checking = 2,
        Business = 3
    }
}
