using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Migrations;

[Migration(25032023_1)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table("Order")
            .WithColumn("Id").AsInt64().PrimaryKey()
            .WithColumn("OrderState").AsInt32()
            .WithColumn("Customer").AsCustom("jsonb")
            .WithColumn("Goods").AsCustom("jsonb")
            .WithColumn("OrderedAt").AsDateTime()
            .WithColumn("OrderType").AsInt32()
            .WithColumn("TotalPrice").AsDecimal()
            .WithColumn("TotalWeight").AsDouble()
            .WithColumn("ItemsCount").AsInt32();
    }

    public override void Down()
    {
        Delete.Table("Order");
    }
}