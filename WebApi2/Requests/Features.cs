using Microsoft.EntityFrameworkCore;

public class Features
{
    public static async Task<List<Feature>> LoadFromDb()
    {
        using var db = new Db();

        var features = await db.Features.ToListAsync();

        return features;
    }
}

public class Feature
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Descr { get; set; }
    public string? Icon { get; set; }
}
