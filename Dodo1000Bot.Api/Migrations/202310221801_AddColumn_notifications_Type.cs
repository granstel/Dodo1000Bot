using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202310221801)]
public class AddColumn_notifications_Type: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE notifications 
            ADD COLUMN `Type` TINYINT(2) NOT NULL DEFAULT 0 AFTER `Id`,
            ADD INDEX `Type` (`Type` ASC) VISIBLE;
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}