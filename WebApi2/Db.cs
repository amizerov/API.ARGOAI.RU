using AmSecrets;
using Microsoft.EntityFrameworkCore;

public class Db : DbContext
{
    public Db() : base(new DbContextOptionsBuilder<Db>()
        .UseSqlServer(Secrets.SqlConnectionString)
        .Options)
    {
    }
    public DbSet<Feature> Features { get; set; } = null!;
}
