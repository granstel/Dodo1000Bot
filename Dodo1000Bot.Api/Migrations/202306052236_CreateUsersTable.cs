using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202306052236)]
public class CreateUsersTable: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            CREATE TABLE IF NOT EXISTS users (
                `Id` INT NOT NULL AUTO_INCREMENT,
                `MessengerUserId` VARCHAR(45) NOT NULL,
                `MessengerType` INT NOT NULL,
                `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (`Id`),
                UNIQUE INDEX `Id_UNIQUE` (`Id` ASC) VISIBLE);
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}