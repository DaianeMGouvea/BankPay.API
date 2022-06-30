using BankPay.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BankPay.API.Data
{
    public class BankPayApiContext : DbContext
    {
        public BankPayApiContext(DbContextOptions<BankPayApiContext> options) : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Account>? Accounts { get; set; }
        public DbSet<OcurrenceRecord>? OccurrenceRecords { get; set; }

    }
}
