using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202308292353)]
public class AddColumn_Users_IsAdmin: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE users 
            ADD COLUMN `IsAdmin` BIT NOT NULL DEFAULT 0;
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}