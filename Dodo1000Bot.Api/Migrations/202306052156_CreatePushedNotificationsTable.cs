using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202306052156)]
public class CreatePushedNotificationsTable: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            CREATE TABLE IF NOT EXISTS pushed_notifications (
              `Id` INT NOT NULL AUTO_INCREMENT,
              `NotificationId` INT NULL,
              `PushedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
              `UserId` INT NOT NULL,
              PRIMARY KEY (`Id`),
              UNIQUE INDEX `Id_UNIQUE` (`Id` ASC) VISIBLE);
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}