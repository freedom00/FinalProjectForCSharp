using System.Data.Entity;

namespace FinalProjectForCSharp
{
    public class OwnersCarsContext : DbContext
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>().HasRequired<Owner>(c => c.CurrentOwner).WithMany(c => c.CarsInGarage).HasForeignKey<int>(c => c.OwnerId);
        }
    }
}