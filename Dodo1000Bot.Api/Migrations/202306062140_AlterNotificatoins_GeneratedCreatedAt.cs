using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202306062140)]
public class AlterNotificatoins_GeneratedCreatedAt: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE notifications 
                CHANGE COLUMN `CreatedAt` `CreatedAt` DATETIME NOT NULL DEFAULT now();
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}