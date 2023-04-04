using FluentMigrator;
using Microsoft.Extensions.Options;

namespace Ozon.Route256.Five.OrderService.Services.Database.Migrator;

public abstract class BucketedMigration : Migration
{
    private readonly MigratorConfiguration _migratorConfig;

    protected BucketedMigration(IOptions<MigratorConfiguration> migratorConfiguration)
    {
        _migratorConfig = migratorConfiguration.Value;
        if (_migratorConfig.BucketsInShard is not { Length: > 0 })
            throw new ArgumentException(
                "Не указаны схемы бакетов, хотя миграция ожидала их",
                nameof(_migratorConfig.BucketsInShard));
    }

    public override void Up()
    {
        foreach (var bucketSchema in BucketSchemas)
        {
            if (!Schema.Schema(bucketSchema).Exists())
                Create.Schema(bucketSchema);
            UpBucketed(bucketSchema);
        }
    }
    
    public override void Down()
    {
        foreach (var bucketSchema in BucketSchemas)
            DownBucketed(bucketSchema);
    }

    
    protected abstract void UpBucketed(string bucketSchema);
    
    protected abstract void DownBucketed(string bucketSchema);

    protected string[] BucketSchemas => _migratorConfig.BucketsInShard!;
}