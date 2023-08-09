using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202308090910)]
public class DropUniqueIndex: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE snapshots 
            DROP INDEX `IX_snapshots_Name`;
        ");
    }

    public override void Down()
    {
        Delete.Table("countries");
    }
}