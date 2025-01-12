using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankAccountAssignment.API.Modules.Data;

namespace BankAccountAssignment.API.Modules.BankAccountModule.Model.BankAccountValidations
{

    public class ValidateCategoryName : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Category name is required.");
            }

            var categoryName = value.ToString();
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
            if (dbContext == null)
            {
                throw new InvalidOperationException("Database context is not available.");
            }

            // Check if the category exists
            var categoryExists = dbContext.Categories.Any(c => c.CategoryName.ToLower() == categoryName.ToLower());
            if (!categoryExists)
            {
                return new ValidationResult($"Category '{categoryName}' does not exist.");
            }

            return ValidationResult.Success;
        }
    }
}


