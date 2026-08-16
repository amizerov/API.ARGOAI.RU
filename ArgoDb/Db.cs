using AmSecrets;
using Microsoft.EntityFrameworkCore;

namespace ArgoDb;

public class Db : DbContext
{
    public DbSet<ChatHistoryEntry> ChatHistory { get; set; } = null!;
    public DbSet<Feature> Features { get; set; } = null!;

    public Db() : base(new DbContextOptionsBuilder<Db>()
        .UseSqlServer(Secrets.SqlConnectionString)
        .Options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.ToTable("Features");
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<ChatHistoryEntry>(entity =>
        {
            entity.ToTable("ChatHistory");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SessionKey).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Role).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<ChatHistoryEntry>()
            .HasIndex(x => new { x.SessionKey, x.CreatedAt });
    }
}

public class Feature
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Descr { get; set; }
    public string? Icon { get; set; }
}

public class ChatHistoryEntry
{
    public int Id { get; set; }
    public string SessionKey { get; set; } = "default";
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public static class ChatHistoryRepository
{
    public const string DefaultSessionKey = "default";

    public static async Task<List<ChatMessage>> LoadAsync(
        string sessionKey = DefaultSessionKey,
        CancellationToken cancellationToken = default)
    {
        ValidateSessionKey(sessionKey);
        using var db = new Db();

        return await db.ChatHistory
            .AsNoTracking()
            .Where(x => x.SessionKey == sessionKey)
            .OrderBy(x => x.Id)
            .Select(x => new ChatMessage
            {
                Role = x.Role,
                Content = x.Content
            })
            .ToListAsync(cancellationToken);
    }

    public static async Task AppendAsync(
        IEnumerable<ChatMessage> messages,
        string sessionKey = DefaultSessionKey,
        CancellationToken cancellationToken = default)
    {
        ValidateSessionKey(sessionKey);
        using var db = new Db();

        var rows = messages
            .Where(x => !string.IsNullOrWhiteSpace(x.Role) && !string.IsNullOrWhiteSpace(x.Content))
            .Select(x => new ChatHistoryEntry
            {
                SessionKey = sessionKey,
                Role = x.Role.Trim(),
                Content = x.Content,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        db.ChatHistory.AddRange(rows);
        await db.SaveChangesAsync(cancellationToken);
    }

    static void ValidateSessionKey(string sessionKey)
    {
        if (string.IsNullOrWhiteSpace(sessionKey))
            throw new ArgumentException("Session key cannot be empty.", nameof(sessionKey));

        if (sessionKey.Length > 128)
            throw new ArgumentException("Session key cannot be longer than 128 characters.", nameof(sessionKey));
    }
}
