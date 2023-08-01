using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202307312023)]
public class CreateSnapshotsTable: Migration
{
    public override void Up()
    {
        Create.Table("snapshots")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(64).NotNullable().Unique()
            .WithColumn("Data").AsCustom("json").NotNullable()
            .WithColumn("ModifiedAt").AsDateTime2();
    }

    public override void Down()
    {
        Delete.Table("snapshots");
    }
}