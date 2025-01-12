using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using BankAccountAssignment.API.Modules.BankAccountModule.Model;
using BankAccountAssignment.API.Modules.UserModules.Model;
using BankAccountAssignment.API.Modules.BankAccountModule.Model.TransactionModel;

namespace BankAccountAssignment.API.Modules.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ExpiredUserToken> ExpiredUserTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<TransactionStatus> TransactionStatuses { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DatabaseSeeder.Seed(modelBuilder);

            // Email unique constraint
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Configure Foreign Key Relationships between BankAccount and User
            modelBuilder.Entity<BankAccount>()
                .HasOne(b => b.User)
                .WithMany(u => u.BankAccounts)
                .HasForeignKey(b => b.UserID);

            // Configure Sequence for BankAccount
            modelBuilder.HasSequence("BankAccountSequence").HasMin(0);

            base.OnModelCreating(modelBuilder);
        }
        // Method to get next sequence value
        public async Task<int> GetBankAccountSeqNumber()
        {
            SqlParameter result = new SqlParameter("@result", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            string query = "SELECT @result = (NEXT VALUE FOR BankAccountSequence)";
            await Database.ExecuteSqlRawAsync(query, result);

            return (int)result.Value;
        }
    }
}
