﻿using FluentMigrator;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Services.Database.Migrator;

namespace Ozon.Route256.Five.OrderService.Migrations;

[Migration(04042023_2)]
public class ExtendOrderWithIndexAndFields : BucketedMigration 
{
    public ExtendOrderWithIndexAndFields(IOptions<MigratorConfiguration> migratorConfiguration) : base(migratorConfiguration)
    {
    }

    protected override void UpBucketed(string bucketSchema)
    {
        Alter.Table("Order")
            .InSchema(bucketSchema)
            .AddColumn("CustomerId")
            .AsInt32()
            .AddColumn("Region")
            .AsString();
        
        Create.Table("index_region_order")
            .InSchema(bucketSchema)
            .WithColumn("order_id").AsInt64().NotNullable()
            .WithColumn("region").AsString().NotNullable();

        Create.PrimaryKey("pk_index_region_order")
            .OnTable("index_region_order")
            .WithSchema(bucketSchema)
            .Columns("order_id");

        Create.Index("region_index_region_order")
            .OnTable("index_region_order")
            .InSchema(bucketSchema)
            .OnColumn("region");
        
        Create.Table("index_customer_order")
            .InSchema(bucketSchema)
            .WithColumn("order_id").AsInt64().NotNullable()
            .WithColumn("customer_id").AsInt64().NotNullable()
            .WithColumn("order_date").AsDateTime().NotNullable();
        
        Create.PrimaryKey("pk_index_customer_order")
            .OnTable("index_customer_order")
            .WithSchema(bucketSchema)
            .Columns("order_id");
        
        Create.Index("region_index_customer_order")
            .OnTable("index_customer_order")
            .InSchema(bucketSchema)
            .OnColumn("customer_id")
            .Ascending()
            .OnColumn("order_date")
            .Descending();
    }

    protected override void DownBucketed(string bucketSchema)
    {
        Delete
            .Column("CustomerId")
            .Column("Region")
            .FromTable("Order")
            .InSchema(bucketSchema);

        Delete.PrimaryKey("pk_index_region_order")
            .FromTable("index_region_order")
            .InSchema(bucketSchema);

        Delete.Index("region_index_region_order")
            .OnTable("index_region_order")
            .InSchema(bucketSchema);
        
        Delete
            .Table("index_region_order")
            .InSchema(bucketSchema);

        Delete.PrimaryKey("pk_index_customer_order")
            .FromTable("index_customer_order")
            .InSchema(bucketSchema);
        
        Delete.Index("region_index_customer_order")
            .OnTable("index_customer_order")
            .InSchema(bucketSchema);

        Delete
            .Table("index_customer_order")
            .InSchema(bucketSchema);
    }
}