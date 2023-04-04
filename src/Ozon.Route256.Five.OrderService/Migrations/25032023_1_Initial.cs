using FluentMigrator;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Services.Database.Migrator;

namespace Ozon.Route256.Five.OrderService.Migrations;

[Migration(25032023_1_2)]
public class InitialMigration : BucketedMigration
{
    public InitialMigration(IOptions<MigratorConfiguration> migratorConfiguration) : base(migratorConfiguration)
    {
    }
    
    protected override void UpBucketed(string bucketSchema)
    {
        Create.Table("Order")
            .InSchema(bucketSchema)
            .WithColumn("Id").AsInt64().PrimaryKey()
            .WithColumn("PhoneNumber").AsString()
            .WithColumn("OrderState").AsInt32()
            .WithColumn("Customer").AsCustom("jsonb")
            .WithColumn("Goods").AsCustom("jsonb")
            .WithColumn("OrderedAt").AsDateTime()
            .WithColumn("OrderType").AsInt32()
            .WithColumn("TotalPrice").AsDecimal()
            .WithColumn("TotalWeight").AsDouble()
            .WithColumn("ItemsCount").AsInt32();
    }

    protected override void DownBucketed(string bucketSchema)
    {
        Delete.Table("Order").InSchema(bucketSchema);
    }
}