using BankAccountAssignment.API.Modules.BankAccountModule.Model;
using BankAccountAssignment.API.Modules.UserModules.Model;
using BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BankAccountAssignment.API.Modules.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Configure Role Seeding
            modelBuilder.Entity<Role>().HasData(
                RetrieveUserRoles()
            );

            // Configure Category Seeding
            modelBuilder.Entity<Category>().HasData(
                RetrieveAccountCategories()
            );

            // Seeding TransactionTypes table
            modelBuilder.Entity<TransactionType>().HasData(
                RetrieveTransactionTypes()
            );

            // Seeding TransactionStatuses table
            modelBuilder.Entity<TransactionStatus>().HasData(
                RetrieveTransactionStatuses()
            );
        }

        /// <summary>
        /// Retrieve user roles from enum
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Role> RetrieveUserRoles() {
            var roles = Enum.GetValues(typeof(RoleTypeEnum))
                .Cast<RoleTypeEnum>()
                .ToDictionary(t => (int)t, t => t.ToString());
            foreach (var role in roles){
                yield return new Role { Id = role.Key , RoleName = role.Value};
            }
        }
        /// <summary>
        /// Retrieve account categories from enum
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Category> RetrieveAccountCategories() {
            var categories = Enum.GetValues(typeof(CategoryTypeEnum))
                .Cast<CategoryTypeEnum>()
                .ToDictionary(t => (int)t, t => t.ToString());
            foreach (var category in categories){
                yield return new Category { CategoryId = category.Key , CategoryName = category.Value};
            }
        }
        /// <summary>
        /// Retrieve transaction types from enum
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TransactionType> RetrieveTransactionTypes() {
            var types = Enum.GetValues(typeof(TransactionTypeEnum))
                .Cast<TransactionTypeEnum>()
                .ToDictionary(t => (int)t, t => t.ToString());
            foreach (var type in types) {
                yield return new TransactionType { Id = type.Key , Name = type.Value};
            }
        }
        /// <summary>
        /// Retrieve transaction statuses from enum
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TransactionStatus> RetrieveTransactionStatuses() {
            var statuses = Enum.GetValues(typeof(TransactionStatusEnum))
                .Cast<TransactionStatusEnum>()
                .ToDictionary(t => (int)t, t => t.ToString());
            foreach (var status in statuses) {
                yield return new TransactionStatus { Id = status.Key , Name = status.Value};
            }
        }
    }
}
