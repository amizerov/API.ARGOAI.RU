using Microsoft.EntityFrameworkCore;

public class Features
{
    public static async Task<List<ArgoDb.Feature>> LoadFromDb()
    {
        using var db = new ArgoDb.Db();

        var features = await db.Features.ToListAsync();

        return features;
    }
}
