using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models.Contexts
{
    public class TwoPhaseCommitContext : DbContext
    {
        public TwoPhaseCommitContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeState> NodeStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().HasData(
                new Node("Order.API") { Id = new Guid("1744de21-e084-4444-9db7-1bc88861a165") },
                new Node("Stock.API") { Id = new Guid("2d5e1ccf-87b7-486b-ae69-088b626896af") },
                new Node("Payment.API") { Id = new Guid("674984d5-2818-47e4-a97a-9c99b4e4257f") }
                );
        }
    }
}