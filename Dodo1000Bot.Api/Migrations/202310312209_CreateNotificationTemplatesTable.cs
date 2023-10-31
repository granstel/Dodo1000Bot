using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202310312209)]
public class CreateNotificationTemplatesTable: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            CREATE TABLE notification_templates (
              `Id` INT NOT NULL AUTO_INCREMENT,
              `NotificationType` TINYINT(2) NOT NULL,
              `MessengerType` TINYINT(1) NOT NULL,
              `LanguageCode` VARCHAR(2) NOT NULL,
              `Template` VARCHAR(450) NOT NULL,
              PRIMARY KEY (`Id`));
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}