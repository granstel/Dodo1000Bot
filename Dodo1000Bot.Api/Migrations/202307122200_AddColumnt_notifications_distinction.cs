using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202307122200)]
public class AddColumnt_notifications_distinction: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE notifications 
            ADD COLUMN `Distinction` INT NOT NULL AFTER `CreatedAt`,
            ADD UNIQUE INDEX `Distinction_UNIQUE` (`Distinction` ASC) VISIBLE;
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}