using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202307012236)]
public class AddUnique_MessengerUserId_MessengerType: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE users 
            ADD UNIQUE INDEX `unique_messengeruserid_messengertype` (`MessengerUserId` ASC, `MessengerType` ASC) INVISIBLE;
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}