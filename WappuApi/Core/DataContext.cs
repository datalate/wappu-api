using Microsoft.EntityFrameworkCore;
using WappuApi.Core.Program;
using WappuApi.Core.Track;

namespace WappuApi.Core;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        OnBeforeSaving();
        return base.SaveChanges();
    }

    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            if (entry.Entity is not EntityBase)
            {
                return;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.CurrentValues[nameof(EntityBase.CreatedAt)] = DateTime.UtcNow;
                    entry.CurrentValues[nameof(EntityBase.UpdatedAt)] = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.CurrentValues[nameof(EntityBase.UpdatedAt)] = DateTime.UtcNow;
                    break;
            }
        }
    }

    public DbSet<ProgramEntity> Programs { get; protected set; } = null!;

    public DbSet<TrackEntity> Tracks { get; protected set; } = null!;
}
