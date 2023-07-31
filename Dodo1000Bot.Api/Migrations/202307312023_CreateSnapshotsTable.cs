using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202307312023)]
public class CreateSnapshotsTable: Migration
{
    public override void Up()
    {
        Create.Table("payloads")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
            .WithColumn("Name").AsString().NotNullable().Unique()
            .WithColumn("data").AsCustom("json").NotNullable();
    }

    public override void Down()
    {
        Delete.Table("payloads");
    }
}