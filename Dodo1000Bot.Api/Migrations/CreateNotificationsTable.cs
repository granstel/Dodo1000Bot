using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202306052135)]
public class CreateNotificationsTable: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            CREATE TABLE notifications (
              `Id` INT NOT NULL AUTO_INCREMENT,
              `Payload` JSON NOT NULL,
              `CreatedAt` DATETIME NOT NULL,
              PRIMARY KEY (`Id`),
              UNIQUE INDEX `Id_UNIQUE` (`Id` ASC) INVISIBLE);
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}