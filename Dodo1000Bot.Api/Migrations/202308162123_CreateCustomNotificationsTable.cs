using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202308162123)]
public class CreateCustomNotificationsTable: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            CREATE TABLE IF NOT EXISTS custom_notifications (
              `Id` INT NOT NULL AUTO_INCREMENT,
              `Payload` JSON NOT NULL,
              `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
              PRIMARY KEY (`Id`),
              UNIQUE INDEX `Id_UNIQUE` (`Id` ASC) INVISIBLE);
        ");
    }

    public override void Down()
    {
        Delete.Table("custom_notifications");
    }
}